using MediatR;
namespace FitJourney.Application.Features.Sessions.Commands;

public record DeleteSessionCommand(string Id, string UserId) : IRequest<bool>;
