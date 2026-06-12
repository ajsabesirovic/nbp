using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Exercises.Commands;

public record UpdateExerciseCommand(string Id, UpdateExerciseRequest Request) : IRequest<ExerciseDto>;
