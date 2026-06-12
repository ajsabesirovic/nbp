using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Sessions.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Sessions.Handlers;

public class GetSessionsQueryHandler(IWorkoutSessionRepository sessions, IMapper mapper)
    : IRequestHandler<GetSessionsQuery, PagedResult<SessionDto>>
{
    public async Task<PagedResult<SessionDto>> Handle(GetSessionsQuery q, CancellationToken ct)
    {
        var (items, total) = await sessions.GetByUserAsync(q.UserId, q.Page, q.Limit);
        return new PagedResult<SessionDto>(mapper.Map<List<SessionDto>>(items), total, q.Page, q.Limit);
    }
}
