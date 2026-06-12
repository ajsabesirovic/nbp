using MediatR;
using FitJourney.Application.Features.Exercises.Commands;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Exercises.Handlers;

public class DeleteExerciseCommandHandler(IExerciseRepository exercises)
    : IRequestHandler<DeleteExerciseCommand, bool>
{
    public async Task<bool> Handle(DeleteExerciseCommand cmd, CancellationToken ct)
    {
        var deleted = await exercises.DeleteAsync(cmd.Id);
        if (!deleted) throw new KeyNotFoundException($"Exercise {cmd.Id} not found");
        return true;
    }
}
