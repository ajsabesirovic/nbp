using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Progress.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Progress.Handlers;

public class GetMuscleBalanceQueryHandler(IWorkoutSessionRepository sessions)
    : IRequestHandler<GetMuscleBalanceQuery, List<MuscleBalanceDto>>
{
    public async Task<List<MuscleBalanceDto>> Handle(GetMuscleBalanceQuery q, CancellationToken ct)
    {
        var from = DateTime.UtcNow.AddDays(-7 * q.Weeks);
        var results = await sessions.GetMuscleBalanceAsync(q.UserId, from);
        return results.Select(r => new MuscleBalanceDto(r.Muscle, r.Sets, r.VolumeKg)).ToList();
    }
}
