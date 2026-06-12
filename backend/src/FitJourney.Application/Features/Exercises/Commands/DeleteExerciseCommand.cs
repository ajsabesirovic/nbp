using MediatR;
namespace FitJourney.Application.Features.Exercises.Commands;

public record DeleteExerciseCommand(string Id) : IRequest<bool>;
