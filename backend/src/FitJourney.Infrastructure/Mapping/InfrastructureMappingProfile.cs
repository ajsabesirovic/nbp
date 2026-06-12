using AutoMapper;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Mapping;

public class InfrastructureMappingProfile : Profile
{
    private static T SafeParse<T>(string value, T fallback) where T : struct, Enum =>
        Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : fallback;

    private static List<MuscleGroup> ParseMuscles(List<string> values) =>
        values.Select(m => (valid: Enum.TryParse<MuscleGroup>(m, ignoreCase: true, out var mg), mg))
              .Where(x => x.valid).Select(x => x.mg).ToList();

    public InfrastructureMappingProfile()
    {

        CreateMap<UserDocument, User>()
            .ForMember(d => d.Role, o => o.MapFrom(s => SafeParse(s.Role, UserRole.user)));
        CreateMap<User, UserDocument>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
        CreateMap<UserProfileDocument, UserProfile>();
        CreateMap<UserProfile, UserProfileDocument>();

        CreateMap<ExerciseDocument, Exercise>()
            .ForMember(d => d.Type, o => o.MapFrom(s => SafeParse(s.Type, ExerciseType.strength)))
            .ForMember(d => d.PrimaryMuscles, o => o.MapFrom(s => ParseMuscles(s.PrimaryMuscles)))
            .ForMember(d => d.SecondaryMuscles, o => o.MapFrom(s => ParseMuscles(s.SecondaryMuscles)));
        CreateMap<Exercise, ExerciseDocument>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.PrimaryMuscles, o => o.MapFrom(s => s.PrimaryMuscles.Select(m => m.ToString()).ToList()))
            .ForMember(d => d.SecondaryMuscles, o => o.MapFrom(s => s.SecondaryMuscles.Select(m => m.ToString()).ToList()));

        CreateMap<WorkoutPlanDocument, WorkoutPlan>()
            .ForMember(d => d.Level, o => o.MapFrom(s => SafeParse(s.Level, PlanLevel.beginner)))
            .ForMember(d => d.Goal, o => o.MapFrom(s => SafeParse(s.Goal, PlanGoal.general_fitness)))
            .ForMember(d => d.Visibility, o => o.MapFrom(s => SafeParse(s.Visibility, Visibility.@private)));
        CreateMap<WorkoutPlan, WorkoutPlanDocument>()
            .ForMember(d => d.Level, o => o.MapFrom(s => s.Level.ToString()))
            .ForMember(d => d.Goal, o => o.MapFrom(s => s.Goal.ToString()))
            .ForMember(d => d.Visibility, o => o.MapFrom(s => s.Visibility.ToString()));

        CreateMap<PlanDayDocument, WorkoutPlanDay>();
        CreateMap<WorkoutPlanDay, PlanDayDocument>();
        CreateMap<PlanExerciseDocument, PlanExercise>();
        CreateMap<PlanExercise, PlanExerciseDocument>();

        CreateMap<WorkoutSessionDocument, WorkoutSession>();
        CreateMap<WorkoutSession, WorkoutSessionDocument>();
        CreateMap<PerformedExerciseDocument, PerformedExercise>();
        CreateMap<PerformedExercise, PerformedExerciseDocument>();
        CreateMap<SessionSetDocument, SessionSet>();
        CreateMap<SessionSet, SessionSetDocument>();

        CreateMap<PersonalRecordDocument, PersonalRecord>();
        CreateMap<PersonalRecord, PersonalRecordDocument>();

        CreateMap<RefreshTokenDocument, RefreshToken>();
        CreateMap<RefreshToken, RefreshTokenDocument>();

        CreateMap<BodyMeasurementDocument, BodyMeasurement>();
        CreateMap<BodyMeasurement, BodyMeasurementDocument>();

        CreateMap<ProgressPhotoDocument, ProgressPhoto>();
        CreateMap<ProgressPhoto, ProgressPhotoDocument>();

        CreateMap<MessageDocument, Message>();
        CreateMap<Message, MessageDocument>();

        CreateMap<NotificationDocument, Notification>();
        CreateMap<Notification, NotificationDocument>();

        CreateMap<RequestLogDocument, RequestLog>();
        CreateMap<RequestLog, RequestLogDocument>();

        CreateMap<TrainerProfileDocument, TrainerProfile>();
        CreateMap<TrainerProfile, TrainerProfileDocument>();

        CreateMap<PlanAssignmentDocument, PlanAssignment>();
        CreateMap<PlanAssignment, PlanAssignmentDocument>();
    }
}
