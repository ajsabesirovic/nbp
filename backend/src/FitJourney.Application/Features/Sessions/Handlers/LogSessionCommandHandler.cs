using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Sessions.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Sessions.Handlers;

public class LogSessionCommandHandler(
    IWorkoutSessionRepository sessions,
    IPersonalRecordRepository prs,
    IExerciseRepository exercises,
    INotificationRepository notifications,
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    IUserRepository users,
    IMapper mapper)
    : IRequestHandler<LogSessionCommand, SessionDto>
{
    public async Task<SessionDto> Handle(LogSessionCommand cmd, CancellationToken ct)
    {
        var req = cmd.Request;
        var exerciseEntities = mapper.Map<List<PerformedExercise>>(req.Exercises);

        double totalVolume = 0;
        int completedSets = 0;
        foreach (var ex in exerciseEntities)
        {
            foreach (var s in ex.Sets)
            {
                if (s.Completed)
                {
                    completedSets++;
                    if (s.WeightKg.HasValue && s.Reps.HasValue)
                        totalVolume += s.WeightKg.Value * s.Reps.Value;
                }
            }
        }
        int durationSec = (int)(req.EndedAt - req.StartedAt).TotalSeconds;

        var session = new WorkoutSession
        {
            UserId = cmd.UserId,
            PlanId = req.PlanId,
            StartedAt = req.StartedAt,
            EndedAt = req.EndedAt,
            Exercises = exerciseEntities,
            Notes = req.Notes,
            Feeling = req.Feeling,
            TotalVolumeKg = totalVolume,
            CompletedSets = completedSets,
            DurationSec = durationSec,
            CreatedAt = DateTime.UtcNow,
        };

        var created = await sessions.CreateAsync(session);

        var exerciseNameCache = new Dictionary<string, string>();
        async Task<string> ResolveName(string exerciseId, string? snapshot)
        {
            if (!string.IsNullOrEmpty(snapshot)) return snapshot;
            if (exerciseNameCache.TryGetValue(exerciseId, out var cached)) return cached;
            var e = await exercises.GetByIdAsync(exerciseId);
            var name = e?.Name ?? exerciseId;
            exerciseNameCache[exerciseId] = name;
            return name;
        }

        async Task TryUpsertPr(string exerciseId, string exerciseName, string type, double weight, int reps, double orm, string title, string body)
        {
            var current = await prs.GetByUserAndExerciseAsync(cmd.UserId, exerciseId, type);
            bool beats = type switch
            {
                "1rm" => current == null || orm > current.OneRepMax,
                "5rm" => current == null || weight > current.WeightKg,
                "reps" => current == null || reps > current.Reps,
                _ => false,
            };
            if (!beats) return;

            await prs.UpsertAsync(new PersonalRecord
            {
                Id = current?.Id ?? string.Empty,
                UserId = cmd.UserId,
                ExerciseId = exerciseId,
                ExerciseName = exerciseName,
                Type = type,
                WeightKg = weight,
                Reps = reps,
                OneRepMax = orm,
                SessionId = created.Id,
                AchievedAt = req.StartedAt,
                CreatedAt = current?.CreatedAt ?? DateTime.UtcNow,
            });

            await notifications.CreateAsync(new Notification
            {
                UserId = cmd.UserId,
                Type = "pr",
                Title = title,
                Body = body,
                Link = "/progress",
                CreatedAt = DateTime.UtcNow,
            });
        }

        var setsByExercise = new Dictionary<string, (string? NameSnapshot, List<(double Weight, int Reps)> Sets)>();
        foreach (var ex in req.Exercises)
        {
            foreach (var s in ex.Sets)
            {
                if (!s.Completed || !s.WeightKg.HasValue || !s.Reps.HasValue) continue;
                int reps = (int)s.Reps.Value;
                if (reps <= 0) continue;
                if (!setsByExercise.TryGetValue(ex.ExerciseId, out var entry))
                {
                    entry = (ex.NameSnapshot, new List<(double, int)>());
                    setsByExercise[ex.ExerciseId] = entry;
                }
                entry.Sets.Add((s.WeightKg.Value, reps));
            }
        }

        foreach (var (exerciseId, entry) in setsByExercise)
        {
            var exerciseName = await ResolveName(exerciseId, entry.NameSnapshot);

            var best1rm = entry.Sets
                .Select(x => (x.Weight, x.Reps, Orm: x.Weight * (1 + x.Reps / 30.0)))
                .OrderByDescending(x => x.Orm)
                .First();
            await TryUpsertPr(exerciseId, exerciseName, "1rm", best1rm.Weight, best1rm.Reps, best1rm.Orm,
                $"New 1RM PR: {exerciseName}",
                $"Estimated 1RM {Math.Round(best1rm.Orm, 1)} kg ({best1rm.Weight} kg × {best1rm.Reps}).");

            var sets5 = entry.Sets.Where(x => x.Reps >= 5).ToList();
            if (sets5.Count > 0)
            {
                var best5 = sets5.OrderByDescending(x => x.Weight).First();
                await TryUpsertPr(exerciseId, exerciseName, "5rm", best5.Weight, best5.Reps, best5.Weight * (1 + best5.Reps / 30.0),
                    $"New 5RM PR: {exerciseName}",
                    $"{best5.Weight} kg × {best5.Reps} reps.");
            }

            var bestReps = entry.Sets.OrderByDescending(x => x.Reps).First();
            await TryUpsertPr(exerciseId, exerciseName, "reps", bestReps.Weight, bestReps.Reps, bestReps.Weight * (1 + bestReps.Reps / 30.0),
                $"New reps PR: {exerciseName}",
                $"{bestReps.Reps} reps @ {bestReps.Weight} kg.");
        }

        await notifications.CreateAsync(new Notification
        {
            UserId = cmd.UserId,
            Type = "session_logged",
            Title = "Workout logged",
            Body = $"{completedSets} sets · {Math.Round(totalVolume)} kg total volume.",
            Link = "/sessions",
            CreatedAt = DateTime.UtcNow,
        });

        await NotifyIfPlanCycleCompleted(cmd.UserId, req.PlanId);

        return mapper.Map<SessionDto>(created);
    }

    private async Task NotifyIfPlanCycleCompleted(string userId, string? planId)
    {
        if (string.IsNullOrWhiteSpace(planId)) return;
        var plan = await plans.GetByIdAsync(planId);
        if (plan == null) return;

        int expected = Math.Max(1, plan.DurationWeeks * plan.DaysPerWeek);
        long logged = await sessions.CountByUserAndPlanAsync(userId, planId);
        if (logged == 0 || logged % expected != 0) return;

        long cycles = logged / expected;
        await notifications.CreateAsync(new Notification
        {
            UserId = userId,
            Type = "plan_completed",
            Title = $"🎉 Plan completed: {plan.Name}",
            Body = cycles == 1
                ? $"You finished all {expected} sessions of \"{plan.Name}\"! Talk to your trainer about what's next."
                : $"You completed cycle {cycles} of \"{plan.Name}\" ({logged} sessions total).",
            Link = "/plans",
            CreatedAt = DateTime.UtcNow,
        });

        var assignment = await assignments.GetAssignmentAsync(planId, userId);
        if (assignment != null)
        {
            var client = await users.GetByIdAsync(userId);
            var clientName = client?.Name ?? "A client";
            await notifications.CreateAsync(new Notification
            {
                UserId = assignment.AssignedBy,
                Type = "plan_completed",
                Title = $"{clientName} completed a plan",
                Body = $"{clientName} finished \"{plan.Name}\" ({logged} sessions). Review their progress or assign a new plan.",
                Link = "/trainer",
                CreatedAt = DateTime.UtcNow,
            });
        }
    }
}
