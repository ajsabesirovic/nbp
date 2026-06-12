using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Progress.Queries;

public record GetProgressionQuery(string UserId, string ExerciseId) : IRequest<List<ProgressionDto>>;
