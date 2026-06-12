using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Trainer.Queries;

public record GetMyClientsQuery(string TrainerUserId) : IRequest<List<ClientSummaryDto>>;
public record GetClientSummaryQuery(string TrainerUserId, string ClientId) : IRequest<ClientDetailDto>;
public record GetTrainerProfileQuery(string TrainerUserId) : IRequest<TrainerProfileDto>;
public record GetPlanAssignedClientsQuery(string TrainerUserId, string PlanId) : IRequest<List<PlanAssignedClientDto>>;
public record GetClientMeasurementsQuery(string TrainerUserId, string ClientId, int Limit) : IRequest<List<BodyMeasurementDto>>;
public record GetClientSessionsQuery(string TrainerUserId, string ClientId, int Page, int Limit) : IRequest<PagedResult<SessionDto>>;
public record GetClientSessionByIdQuery(string TrainerUserId, string ClientId, string SessionId) : IRequest<SessionDto>;
