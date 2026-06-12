using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Profile.Commands;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Profile.Handlers;

public class SetActivePlanCommandHandler(
    IUserRepository users,
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    IMapper mapper)
    : IRequestHandler<SetActivePlanCommand, MeResponse>
{
    public async Task<MeResponse> Handle(SetActivePlanCommand cmd, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(cmd.UserId) ?? throw new KeyNotFoundException("User not found");

        if (string.IsNullOrWhiteSpace(cmd.PlanId))
        {
            user.ActivePlanId = null;
        }
        else
        {
            var plan = await plans.GetByIdAsync(cmd.PlanId)
                ?? throw new KeyNotFoundException($"Plan {cmd.PlanId} not found");

            var allowed = plan.AuthorId == cmd.UserId
                || plan.Visibility == Visibility.@public
                || await assignments.IsActivelyAssignedAsync(plan.Id, cmd.UserId);
            if (!allowed)
                throw new ForbiddenException("You don't have access to this plan.");

            user.ActivePlanId = plan.Id;
        }

        user.UpdatedAt = DateTime.UtcNow;
        var saved = await users.UpdateAsync(user);
        return new MeResponse(mapper.Map<UserDto>(saved));
    }
}
