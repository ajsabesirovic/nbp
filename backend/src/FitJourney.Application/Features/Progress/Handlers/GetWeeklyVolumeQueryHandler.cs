using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Progress.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Progress.Handlers;

public class GetWeeklyVolumeQueryHandler(IWorkoutSessionRepository sessions)
    : IRequestHandler<GetWeeklyVolumeQuery, List<WeeklyVolumeDto>>
{
    public async Task<List<WeeklyVolumeDto>> Handle(GetWeeklyVolumeQuery q, CancellationToken ct)
    {
        var from = DateTime.UtcNow.AddDays(-7 * q.Weeks);
        var results = await sessions.GetWeeklyVolumeAsync(q.UserId, from);
        return results.Select(r => new WeeklyVolumeDto(r.Year, r.Week, r.TotalVolumeKg, r.SessionCount, r.TotalSets)).ToList();
    }
}
