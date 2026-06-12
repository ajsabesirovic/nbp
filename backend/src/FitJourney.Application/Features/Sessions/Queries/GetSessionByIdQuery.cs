using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Sessions.Queries;

public record GetSessionByIdQuery(string Id, string UserId) : IRequest<SessionDto>;
