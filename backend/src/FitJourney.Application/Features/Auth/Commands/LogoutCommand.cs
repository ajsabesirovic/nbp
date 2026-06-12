using MediatR;
namespace FitJourney.Application.Features.Auth.Commands;

public record LogoutCommand(string Token) : IRequest<Unit>;
