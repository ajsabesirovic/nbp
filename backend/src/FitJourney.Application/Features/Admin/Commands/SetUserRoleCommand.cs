using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Admin.Commands;

public record SetUserRoleCommand(string UserId, string Role) : IRequest<AdminUserDto>;
