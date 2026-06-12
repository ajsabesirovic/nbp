using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.Features.Sessions.Commands;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Sessions.Handlers;

public class DeleteSessionCommandHandler(IWorkoutSessionRepository sessions)
    : IRequestHandler<DeleteSessionCommand, bool>
{
    public async Task<bool> Handle(DeleteSessionCommand cmd, CancellationToken ct)
    {
        var existing = await sessions.GetByIdAsync(cmd.Id)
            ?? throw new KeyNotFoundException($"Session {cmd.Id} not found");
        if (existing.UserId != cmd.UserId)
            throw new ForbiddenException("You don't own this session");
        var deleted = await sessions.DeleteAsync(cmd.Id);
        if (!deleted) throw new KeyNotFoundException($"Session {cmd.Id} not found");
        return true;
    }
}
