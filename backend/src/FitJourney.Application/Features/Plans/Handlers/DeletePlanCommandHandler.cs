using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.Features.Plans.Commands;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Plans.Handlers;

public class DeletePlanCommandHandler(IWorkoutPlanRepository plans)
    : IRequestHandler<DeletePlanCommand, bool>
{
    public async Task<bool> Handle(DeletePlanCommand cmd, CancellationToken ct)
    {
        var existing = await plans.GetByIdAsync(cmd.Id)
            ?? throw new KeyNotFoundException($"Plan {cmd.Id} not found");
        if (existing.AuthorId != cmd.UserId)
            throw new ForbiddenException("You are not the author of this plan");
        var deleted = await plans.DeleteAsync(cmd.Id);
        if (!deleted) throw new KeyNotFoundException($"Plan {cmd.Id} not found");
        return true;
    }
}
