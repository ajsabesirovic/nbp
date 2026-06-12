using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.Repositories;
using FitJourney.Infrastructure.Services;
using FitJourney.Infrastructure.Settings;
namespace FitJourney.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoSettings>(config.GetSection("Mongo"));
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>();
        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
        services.AddScoped<IPersonalRecordRepository, PersonalRecordRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IBodyMeasurementRepository, BodyMeasurementRepository>();
        services.AddScoped<IProgressPhotoRepository, ProgressPhotoRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRequestLogRepository, RequestLogRepository>();
        services.AddScoped<ITrainerProfileRepository, TrainerProfileRepository>();
        services.AddScoped<IPlanAssignmentRepository, PlanAssignmentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<FitJourney.Application.Services.IReminderService, FitJourney.Application.Services.ReminderService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<DbSeeder>();

        var cloudinaryUrl = config["Cloudinary:Url"];
        if (!string.IsNullOrWhiteSpace(cloudinaryUrl))
            services.AddSingleton<IPhotoStorage>(new CloudinaryPhotoStorage(cloudinaryUrl));

        return services;
    }
}
