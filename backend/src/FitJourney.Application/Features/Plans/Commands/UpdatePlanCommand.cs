using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Plans.Commands;

public record UpdatePlanCommand(string Id, UpdatePlanRequest Request, string UserId) : IRequest<PlanDto>;
