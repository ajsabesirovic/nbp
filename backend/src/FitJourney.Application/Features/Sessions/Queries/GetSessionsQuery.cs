using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Sessions.Queries;

public record GetSessionsQuery(string UserId, int Page, int Limit) : IRequest<PagedResult<SessionDto>>;
