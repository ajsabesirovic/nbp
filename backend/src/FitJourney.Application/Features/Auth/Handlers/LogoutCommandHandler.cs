using MediatR;
using FitJourney.Application.Features.Auth.Commands;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Auth.Handlers;

public class LogoutCommandHandler(IRefreshTokenRepository tokens) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand cmd, CancellationToken ct)
    {
        await tokens.RevokeAsync(cmd.Token);
        return Unit.Value;
    }
}
