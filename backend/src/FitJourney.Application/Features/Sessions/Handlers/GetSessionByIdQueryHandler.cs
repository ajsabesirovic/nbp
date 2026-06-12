using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Sessions.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Sessions.Handlers;

public class GetSessionByIdQueryHandler(IWorkoutSessionRepository sessions, IMapper mapper)
    : IRequestHandler<GetSessionByIdQuery, SessionDto>
{
    public async Task<SessionDto> Handle(GetSessionByIdQuery q, CancellationToken ct)
    {
        var session = await sessions.GetByIdAsync(q.Id)
            ?? throw new KeyNotFoundException($"Session {q.Id} not found");
        if (session.UserId != q.UserId)
            throw new ForbiddenException("You don't own this session");
        return mapper.Map<SessionDto>(session);
    }
}
