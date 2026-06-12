using MediatR;
namespace FitJourney.Application.Features.Plans.Commands;

public record DeletePlanCommand(string Id, string UserId) : IRequest<bool>;
