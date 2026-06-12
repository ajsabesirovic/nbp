using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class GetClientSessionsQueryHandler(
    ITrainerProfileRepository profiles,
    IWorkoutSessionRepository sessions,
    IMapper mapper)
    : IRequestHandler<GetClientSessionsQuery, PagedResult<SessionDto>>
{
    public async Task<PagedResult<SessionDto>> Handle(GetClientSessionsQuery q, CancellationToken ct)
    {
        await GetClientMeasurementsQueryHandler.EnsureClientOf(profiles, q.TrainerUserId, q.ClientId);
        var (items, total) = await sessions.GetByUserAsync(q.ClientId, q.Page, q.Limit);
        return new PagedResult<SessionDto>(mapper.Map<List<SessionDto>>(items), total, q.Page, q.Limit);
    }
}

public class GetClientSessionByIdQueryHandler(
    ITrainerProfileRepository profiles,
    IWorkoutSessionRepository sessions,
    IMapper mapper)
    : IRequestHandler<GetClientSessionByIdQuery, SessionDto>
{
    public async Task<SessionDto> Handle(GetClientSessionByIdQuery q, CancellationToken ct)
    {
        await GetClientMeasurementsQueryHandler.EnsureClientOf(profiles, q.TrainerUserId, q.ClientId);
        var session = await sessions.GetByIdAsync(q.SessionId);

        if (session == null || session.UserId != q.ClientId)
            throw new KeyNotFoundException($"Session {q.SessionId} not found for this client");
        return mapper.Map<SessionDto>(session);
    }
}
