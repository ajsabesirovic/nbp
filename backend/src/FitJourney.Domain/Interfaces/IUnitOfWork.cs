namespace FitJourney.Domain.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IExerciseRepository Exercises { get; }
    IWorkoutPlanRepository Plans { get; }
    IWorkoutSessionRepository Sessions { get; }
    IPersonalRecordRepository PersonalRecords { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IBodyMeasurementRepository BodyMeasurements { get; }
    IProgressPhotoRepository ProgressPhotos { get; }
    IMessageRepository Messages { get; }
    INotificationRepository Notifications { get; }
    IRequestLogRepository RequestLogs { get; }
    ITrainerProfileRepository TrainerProfiles { get; }
    IPlanAssignmentRepository PlanAssignments { get; }

    Task SaveChangesAsync(CancellationToken ct = default);
}
