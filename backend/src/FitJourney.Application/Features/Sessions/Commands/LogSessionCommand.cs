using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Sessions.Commands;

public record LogSessionCommand(CreateSessionRequest Request, string UserId) : IRequest<SessionDto>;
