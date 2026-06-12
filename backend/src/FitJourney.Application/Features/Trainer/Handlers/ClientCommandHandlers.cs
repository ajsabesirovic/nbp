using MediatR;
using FitJourney.Application.Features.Trainer.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class AddClientCommandHandler(ITrainerProfileRepository profiles, IUserRepository users)
    : IRequestHandler<AddClientCommand, Unit>
{
    public async Task<Unit> Handle(AddClientCommand cmd, CancellationToken ct)
    {

        var trainer = await users.GetByIdAsync(cmd.TrainerUserId)
            ?? throw new KeyNotFoundException($"Trainer {cmd.TrainerUserId} not found");
        if (trainer.Role != UserRole.trainer)
            throw new InvalidOperationException("Clients can only be assigned to a trainer.");

        var client = await users.GetByIdAsync(cmd.ClientUserId)
            ?? throw new KeyNotFoundException($"Client {cmd.ClientUserId} not found");
        if (client.Role == UserRole.admin)
            throw new InvalidOperationException("Cannot add an admin as a client");

        var existingTrainers = await profiles.GetTrainersForClientAsync(cmd.ClientUserId);
        if (existingTrainers.Any(t => t.UserId != cmd.TrainerUserId))
            throw new InvalidOperationException("This user is already a client of another trainer.");

        var profile = await profiles.GetByUserIdAsync(cmd.TrainerUserId);
        if (profile == null)
        {
            var now = DateTime.UtcNow;
            await profiles.UpsertAsync(new TrainerProfile
            {
                UserId = cmd.TrainerUserId,
                ClientIds = [cmd.ClientUserId],
                CreatedAt = now,
                UpdatedAt = now,
            });
        }
        else
        {
            await profiles.AddClientAsync(cmd.TrainerUserId, cmd.ClientUserId);
        }
        return Unit.Value;
    }
}

public class RemoveClientCommandHandler(
    ITrainerProfileRepository profiles,
    IPlanAssignmentRepository assignments,
    IUserRepository users)
    : IRequestHandler<RemoveClientCommand, Unit>
{
    public async Task<Unit> Handle(RemoveClientCommand cmd, CancellationToken ct)
    {
        await profiles.RemoveClientAsync(cmd.TrainerUserId, cmd.ClientUserId);

        var planIds = await assignments.GetActivePlanIdsByTrainerForUserAsync(cmd.TrainerUserId, cmd.ClientUserId);
        await assignments.CancelAllByTrainerForUserAsync(cmd.TrainerUserId, cmd.ClientUserId);

        var client = await users.GetByIdAsync(cmd.ClientUserId);
        if (client != null && client.ActivePlanId != null && planIds.Contains(client.ActivePlanId))
        {
            client.ActivePlanId = null;
            client.UpdatedAt = DateTime.UtcNow;
            await users.UpdateAsync(client);
        }

        return Unit.Value;
    }
}
