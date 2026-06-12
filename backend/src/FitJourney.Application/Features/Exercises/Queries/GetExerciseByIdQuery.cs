using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Exercises.Queries;

public record GetExerciseByIdQuery(string Id) : IRequest<ExerciseDto>;
