using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Progress.Queries;

public record GetWeeklyVolumeQuery(string UserId, int Weeks) : IRequest<List<WeeklyVolumeDto>>;
