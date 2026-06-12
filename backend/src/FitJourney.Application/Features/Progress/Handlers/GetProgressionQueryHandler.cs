using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Progress.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Progress.Handlers;

public class GetProgressionQueryHandler(IWorkoutSessionRepository sessions)
    : IRequestHandler<GetProgressionQuery, List<ProgressionDto>>
{
    public async Task<List<ProgressionDto>> Handle(GetProgressionQuery q, CancellationToken ct)
    {
        var results = await sessions.GetProgressionAsync(q.UserId, q.ExerciseId);
        return results.Select(r => new ProgressionDto(r.Date, r.MaxWeightKg, r.OneRepMax, r.TotalSets, r.TotalVolumeKg)).ToList();
    }
}
