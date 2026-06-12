using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Exercises.Commands;

public record CreateExerciseCommand(CreateExerciseRequest Request, string? CreatedBy) : IRequest<ExerciseDto>;
