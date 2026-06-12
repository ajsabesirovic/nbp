using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Trainer.Commands;

public record AddClientCommand(string TrainerUserId, string ClientUserId) : IRequest<Unit>;
public record RemoveClientCommand(string TrainerUserId, string ClientUserId) : IRequest<Unit>;
public record AssignPlanToClientCommand(string TrainerUserId, string PlanId, string ClientUserId) : IRequest<PlanDto>;
public record UnassignPlanFromClientCommand(string TrainerUserId, string PlanId, string ClientUserId) : IRequest<Unit>;
public record UpdateTrainerProfileCommand(string TrainerUserId, UpdateTrainerProfileRequest Request) : IRequest<TrainerProfileDto>;
public record CreateClientMeasurementCommand(string TrainerUserId, string ClientId, CreateBodyMeasurementRequest Request) : IRequest<BodyMeasurementDto>;
