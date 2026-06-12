using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;

namespace FitJourney.Application.Services;

public class ReminderService(
    IWorkoutSessionRepository sessions,
    IUserRepository users,
    IWorkoutPlanRepository plans,
    INotificationRepository notifications) : IReminderService
{

    private const int MissedDaysThreshold = 3;

    public async Task EvaluateAsync(string userId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var recent = await sessions.GetRecentAsync(userId, 1);
        DateTime? lastWorkout = recent.Count > 0 ? recent[0].StartedAt.Date : null;

        if (lastWorkout.HasValue)
        {
            var daysSince = (today - lastWorkout.Value).Days;
            if (daysSince >= MissedDaysThreshold
                && !await notifications.ExistsByTypeSinceAsync(userId, "missed_workout", today))
            {
                await notifications.CreateAsync(new Notification
                {
                    UserId = userId,
                    Type = "missed_workout",
                    Title = "We miss you! 💪",
                    Body = $"It's been {daysSince} days since your last workout — time to get back to it.",
                    Link = "/sessions/new",
                    CreatedAt = DateTime.UtcNow,
                });
            }
        }

        var user = await users.GetByIdAsync(userId);
        var workedOutToday = lastWorkout == today;
        if (user?.ActivePlanId != null
            && !workedOutToday
            && !await notifications.ExistsByTypeSinceAsync(userId, "workout_reminder", today))
        {
            var plan = await plans.GetByIdAsync(user.ActivePlanId);
            await notifications.CreateAsync(new Notification
            {
                UserId = userId,
                Type = "workout_reminder",
                Title = "Time for today's workout",
                Body = plan != null
                    ? $"Your plan \"{plan.Name}\" is waiting — log today's session."
                    : "Log today's session to stay on track.",
                Link = "/sessions/new",
                CreatedAt = DateTime.UtcNow,
            });
        }
    }
}
