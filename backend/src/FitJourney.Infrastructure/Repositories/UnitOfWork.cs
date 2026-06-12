using FitJourney.Domain.Interfaces;
namespace FitJourney.Infrastructure.Repositories;

public class UnitOfWork(
    IUserRepository users,
    IExerciseRepository exercises,
    IWorkoutPlanRepository plans,
    IWorkoutSessionRepository sessions,
    IPersonalRecordRepository personalRecords,
    IRefreshTokenRepository refreshTokens,
    IBodyMeasurementRepository bodyMeasurements,
    IProgressPhotoRepository progressPhotos,
    IMessageRepository messages,
    INotificationRepository notifications,
    IRequestLogRepository requestLogs,
    ITrainerProfileRepository trainerProfiles,
    IPlanAssignmentRepository planAssignments) : IUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IExerciseRepository Exercises { get; } = exercises;
    public IWorkoutPlanRepository Plans { get; } = plans;
    public IWorkoutSessionRepository Sessions { get; } = sessions;
    public IPersonalRecordRepository PersonalRecords { get; } = personalRecords;
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;
    public IBodyMeasurementRepository BodyMeasurements { get; } = bodyMeasurements;
    public IProgressPhotoRepository ProgressPhotos { get; } = progressPhotos;
    public IMessageRepository Messages { get; } = messages;
    public INotificationRepository Notifications { get; } = notifications;
    public IRequestLogRepository RequestLogs { get; } = requestLogs;
    public ITrainerProfileRepository TrainerProfiles { get; } = trainerProfiles;
    public IPlanAssignmentRepository PlanAssignments { get; } = planAssignments;

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
}
