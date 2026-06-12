using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Exercises.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Exercises.Handlers;

public class CreateExerciseCommandHandler(IExerciseRepository exercises, IMapper mapper)
    : IRequestHandler<CreateExerciseCommand, ExerciseDto>
{
    public async Task<ExerciseDto> Handle(CreateExerciseCommand cmd, CancellationToken ct)
    {
        var entity = new Exercise
        {
            Name = cmd.Request.Name,
            Type = Enum.Parse<ExerciseType>(cmd.Request.Type, ignoreCase: true),
            PrimaryMuscles = cmd.Request.PrimaryMuscles
                .Select(m => Enum.Parse<MuscleGroup>(m, ignoreCase: true)).ToList(),
            SecondaryMuscles = (cmd.Request.SecondaryMuscles ?? [])
                .Select(m => Enum.Parse<MuscleGroup>(m, ignoreCase: true)).ToList(),
            Category = cmd.Request.Category,
            Equipment = cmd.Request.Equipment,
            Difficulty = cmd.Request.Difficulty,
            Description = cmd.Request.Description,
            Instructions = cmd.Request.Instructions,
            ImageUrl = cmd.Request.ImageUrl,
            VideoUrl = cmd.Request.VideoUrl,
            IsCustom = cmd.CreatedBy != null,
            CreatedBy = cmd.CreatedBy,
            CreatedAt = DateTime.UtcNow,
        };
        var created = await exercises.CreateAsync(entity);
        return mapper.Map<ExerciseDto>(created);
    }
}
