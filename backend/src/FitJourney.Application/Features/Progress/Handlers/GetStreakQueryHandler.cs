using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Progress.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Progress.Handlers;

public class GetStreakQueryHandler(IWorkoutSessionRepository sessions)
    : IRequestHandler<GetStreakQuery, StreakDto>
{
    public async Task<StreakDto> Handle(GetStreakQuery q, CancellationToken ct)
    {
        var allSessions = await sessions.GetForStreakAsync(q.UserId);
        if (allSessions.Count == 0)
            return new StreakDto(0, 0, 0, null);

        var workoutDates = allSessions
            .Select(s => s.StartedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        int totalWorkoutDays = workoutDates.Count;
        DateTime? lastWorkoutDate = workoutDates.FirstOrDefault();

        int currentStreak = 0;
        var today = DateTime.UtcNow.Date;
        var checkDate = today;

        if (workoutDates.Contains(today) || workoutDates.Contains(today.AddDays(-1)))
        {
            if (!workoutDates.Contains(today))
                checkDate = today.AddDays(-1);

            while (workoutDates.Contains(checkDate))
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }
        }

        int longestStreak = 0;
        int streak = 1;
        var sortedDates = workoutDates.OrderBy(d => d).ToList();
        for (int i = 1; i < sortedDates.Count; i++)
        {
            if ((sortedDates[i] - sortedDates[i - 1]).TotalDays == 1)
            {
                streak++;
            }
            else
            {
                longestStreak = Math.Max(longestStreak, streak);
                streak = 1;
            }
        }
        longestStreak = Math.Max(longestStreak, streak);

        return new StreakDto(currentStreak, longestStreak, totalWorkoutDays, lastWorkoutDate);
    }
}
