using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Plans.Queries;

public record GetPlanByIdQuery(string Id, string RequesterId, bool IsAdmin) : IRequest<PlanDto>;
