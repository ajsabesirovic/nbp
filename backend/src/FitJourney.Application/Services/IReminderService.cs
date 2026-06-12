namespace FitJourney.Application.Services;

public interface IReminderService
{
    Task EvaluateAsync(string userId, CancellationToken ct = default);
}
