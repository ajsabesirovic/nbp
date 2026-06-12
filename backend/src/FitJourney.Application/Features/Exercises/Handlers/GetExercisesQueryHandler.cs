using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Exercises.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Exercises.Handlers;

public class GetExercisesQueryHandler(IExerciseRepository exercises, IMapper mapper)
    : IRequestHandler<GetExercisesQuery, PagedResult<ExerciseDto>>
{
    public async Task<PagedResult<ExerciseDto>> Handle(GetExercisesQuery q, CancellationToken ct)
    {
        var (items, total) = await exercises.GetAllAsync(q.Search, q.Type, q.Muscle, q.Page, q.Limit);
        return new PagedResult<ExerciseDto>(mapper.Map<List<ExerciseDto>>(items), total, q.Page, q.Limit);
    }
}
