using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Auth.Commands;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler(IUnitOfWork uow, IJwtService jwt, IMapper mapper)
    : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var stored = await uow.RefreshTokens.GetByTokenAsync(cmd.Token)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");
        if (stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired or revoked");

        var user = await uow.Users.GetByIdAsync(stored.UserId)
            ?? throw new UnauthorizedAccessException("User not found");

        var newRefresh = jwt.GenerateRefreshToken();
        await uow.RefreshTokens.RevokeAsync(cmd.Token, newRefresh);
        await uow.RefreshTokens.CreateAsync(new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = newRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        });
        await uow.SaveChangesAsync(ct);

        var access = jwt.GenerateAccessToken(user);
        return new AuthResponse(access, newRefresh, mapper.Map<UserDto>(user));
    }
}
