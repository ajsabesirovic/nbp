using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Progress.Queries;

public record GetStreakQuery(string UserId) : IRequest<StreakDto>;
