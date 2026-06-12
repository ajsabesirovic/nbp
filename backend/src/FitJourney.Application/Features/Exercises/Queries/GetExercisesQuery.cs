using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Exercises.Queries;

public record GetExercisesQuery(string? Search, string? Type, string? Muscle, int Page, int Limit)
    : IRequest<PagedResult<ExerciseDto>>;
