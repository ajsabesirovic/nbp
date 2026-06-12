using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Progress.Queries;

public record GetMuscleBalanceQuery(string UserId, int Weeks) : IRequest<List<MuscleBalanceDto>>;
