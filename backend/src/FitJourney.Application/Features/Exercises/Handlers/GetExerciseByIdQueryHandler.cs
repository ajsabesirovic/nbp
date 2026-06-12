using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Exercises.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Exercises.Handlers;

public class GetExerciseByIdQueryHandler(IExerciseRepository exercises, IMapper mapper)
    : IRequestHandler<GetExerciseByIdQuery, ExerciseDto>
{
    public async Task<ExerciseDto> Handle(GetExerciseByIdQuery q, CancellationToken ct)
    {
        var exercise = await exercises.GetByIdAsync(q.Id)
            ?? throw new KeyNotFoundException($"Exercise {q.Id} not found");
        return mapper.Map<ExerciseDto>(exercise);
    }
}
