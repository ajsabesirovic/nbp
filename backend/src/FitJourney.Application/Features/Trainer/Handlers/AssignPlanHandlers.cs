using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Commands;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class AssignPlanToClientCommandHandler(
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    ITrainerProfileRepository profiles,
    IUserRepository users,
    IMapper mapper)
    : IRequestHandler<AssignPlanToClientCommand, PlanDto>
{
    public async Task<PlanDto> Handle(AssignPlanToClientCommand cmd, CancellationToken ct)
    {
        var plan = await plans.GetByIdAsync(cmd.PlanId)
            ?? throw new KeyNotFoundException($"Plan {cmd.PlanId} not found");

        if (plan.AuthorId != cmd.TrainerUserId && plan.Visibility != Visibility.@public)
            throw new ForbiddenException("You can only assign your own plans or public plans");

        var profile = await profiles.GetByUserIdAsync(cmd.TrainerUserId)
            ?? throw new InvalidOperationException("Trainer has no clients");
        if (!profile.ClientIds.Contains(cmd.ClientUserId))
            throw new ForbiddenException("Client is not in your client list");

        await assignments.AssignAsync(cmd.PlanId, cmd.ClientUserId, cmd.TrainerUserId);

        var client = await users.GetByIdAsync(cmd.ClientUserId);
        if (client != null)
        {
            client.ActivePlanId = cmd.PlanId;
            client.UpdatedAt = DateTime.UtcNow;
            await users.UpdateAsync(client);
        }

        return mapper.Map<PlanDto>(plan);
    }
}

public class UnassignPlanFromClientCommandHandler(
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    IUserRepository users)
    : IRequestHandler<UnassignPlanFromClientCommand, Unit>
{
    public async Task<Unit> Handle(UnassignPlanFromClientCommand cmd, CancellationToken ct)
    {
        _ = await plans.GetByIdAsync(cmd.PlanId)
            ?? throw new KeyNotFoundException($"Plan {cmd.PlanId} not found");

        var removed = await assignments.UnassignAsync(cmd.PlanId, cmd.ClientUserId, cmd.TrainerUserId);
        if (!removed)
            throw new ForbiddenException("You can only unassign plans you assigned to this client.");

        var client = await users.GetByIdAsync(cmd.ClientUserId);
        if (client != null && client.ActivePlanId == cmd.PlanId)
        {
            client.ActivePlanId = null;
            client.UpdatedAt = DateTime.UtcNow;
            await users.UpdateAsync(client);
        }

        return Unit.Value;
    }
}
