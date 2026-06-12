using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Auth.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Auth.Handlers;

public class RegisterCommandHandler(
    IUserRepository users,
    IJwtService jwt,
    IRefreshTokenRepository tokens,
    INotificationRepository notifications,
    IMapper mapper)
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var existing = await users.GetByEmailAsync(cmd.Request.Email);
        if (existing != null) throw new InvalidOperationException("Email already in use");

        var role = UserRole.user;
        if (!string.IsNullOrWhiteSpace(cmd.Request.Role)
            && string.Equals(cmd.Request.Role, "trainer", StringComparison.OrdinalIgnoreCase))
        {
            role = UserRole.trainer;
        }

        var user = new User
        {
            Name = cmd.Request.Name,
            Email = cmd.Request.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(cmd.Request.Password, 12),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var created = await users.CreateAsync(user);

        await notifications.CreateAsync(new Notification
        {
            UserId = created.Id,
            Type = "welcome",
            Title = "Welcome to FitJourney!",
            Body = "Browse the exercise library, follow a plan, and log your first workout.",
            Link = "/plans",
            CreatedAt = DateTime.UtcNow,
        });

        return await IssueTokens(created);
    }

    private async Task<AuthResponse> IssueTokens(User user)
    {
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
