using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Plans.Commands;

public record CreatePlanCommand(CreatePlanRequest Request, string AuthorId, string AuthorName) : IRequest<PlanDto>;
