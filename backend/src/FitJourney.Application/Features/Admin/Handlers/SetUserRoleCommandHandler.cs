using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Commands;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Admin.Handlers;

public class SetUserRoleCommandHandler(IUserRepository users, IMapper mapper)
    : IRequestHandler<SetUserRoleCommand, AdminUserDto>
{
    public async Task<AdminUserDto> Handle(SetUserRoleCommand cmd, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(cmd.UserId)
            ?? throw new KeyNotFoundException($"User {cmd.UserId} not found");

        if (!Enum.TryParse<UserRole>(cmd.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role '{cmd.Role}'");

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        var updated = await users.UpdateAsync(user);
        return mapper.Map<AdminUserDto>(updated);
    }
}
