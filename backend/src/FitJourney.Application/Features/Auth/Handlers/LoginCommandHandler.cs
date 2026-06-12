using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Auth.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Auth.Handlers;

public class LoginCommandHandler(IUserRepository users, IJwtService jwt, IRefreshTokenRepository tokens, IMapper mapper)
    : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(cmd.Request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");
        if (!BCrypt.Net.BCrypt.Verify(cmd.Request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var access = jwt.GenerateAccessToken(user);
        var rawRefresh = jwt.GenerateRefreshToken();
        await tokens.CreateAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = rawRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        });
        return new AuthResponse(access, rawRefresh, mapper.Map<UserDto>(user));
    }
}
