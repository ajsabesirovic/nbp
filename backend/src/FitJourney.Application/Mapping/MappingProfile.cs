using AutoMapper;
using FitJourney.Application.DTOs;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
namespace FitJourney.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
        CreateMap<User, AdminUserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
        CreateMap<RequestLog, RequestLogDto>();
        CreateMap<TrainerProfile, TrainerProfileDto>();
        CreateMap<UserProfile, UserProfileDto>();
        CreateMap<UserProfileDto, UserProfile>();

        CreateMap<Exercise, ExerciseDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.PrimaryMuscles, o => o.MapFrom(s => s.PrimaryMuscles.Select(m => m.ToString()).ToList()))
            .ForMember(d => d.SecondaryMuscles, o => o.MapFrom(s => s.SecondaryMuscles.Select(m => m.ToString()).ToList()));

        CreateMap<WorkoutPlan, PlanDto>()
            .ForMember(d => d.Level, o => o.MapFrom(s => s.Level.ToString()))
            .ForMember(d => d.Goal, o => o.MapFrom(s => s.Goal.ToString()))
            .ForMember(d => d.Visibility, o => o.MapFrom(s => s.Visibility.ToString()));

        CreateMap<WorkoutPlanDay, PlanDayDto>();
        CreateMap<PlanExercise, PlanExerciseDto>();
        CreateMap<PlanDayDto, WorkoutPlanDay>();
        CreateMap<PlanExerciseDto, PlanExercise>();

        CreateMap<WorkoutSession, SessionDto>();
        CreateMap<PerformedExercise, PerformedExerciseDto>();
        CreateMap<SessionSet, SessionSetDto>();
        CreateMap<PerformedExerciseDto, PerformedExercise>();
        CreateMap<SessionSetDto, SessionSet>();

        CreateMap<PersonalRecord, PersonalRecordDto>();

        CreateMap<BodyMeasurement, BodyMeasurementDto>()
            .ForCtorParam(nameof(BodyMeasurementDto.RecordedAt), o => o.MapFrom(s => s.Date));
        CreateMap<ProgressPhoto, ProgressPhotoDto>();
        CreateMap<Message, MessageDto>();
        CreateMap<Notification, NotificationDto>();
    }
}
