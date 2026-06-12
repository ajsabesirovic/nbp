using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Exercises.Commands;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Exercises.Handlers;

public class UpdateExerciseCommandHandler(IExerciseRepository exercises, IMapper mapper)
    : IRequestHandler<UpdateExerciseCommand, ExerciseDto>
{
    public async Task<ExerciseDto> Handle(UpdateExerciseCommand cmd, CancellationToken ct)
    {
        var existing = await exercises.GetByIdAsync(cmd.Id)
            ?? throw new KeyNotFoundException($"Exercise {cmd.Id} not found");

        if (cmd.Request.Name != null) existing.Name = cmd.Request.Name;
        if (cmd.Request.Type != null) existing.Type = Enum.Parse<ExerciseType>(cmd.Request.Type, ignoreCase: true);
        if (cmd.Request.PrimaryMuscles != null)
            existing.PrimaryMuscles = cmd.Request.PrimaryMuscles.Select(m => Enum.Parse<MuscleGroup>(m, ignoreCase: true)).ToList();
        if (cmd.Request.SecondaryMuscles != null)
            existing.SecondaryMuscles = cmd.Request.SecondaryMuscles.Select(m => Enum.Parse<MuscleGroup>(m, ignoreCase: true)).ToList();
        if (cmd.Request.Category != null) existing.Category = cmd.Request.Category;
        if (cmd.Request.Equipment != null) existing.Equipment = cmd.Request.Equipment;
        if (cmd.Request.Difficulty.HasValue) existing.Difficulty = cmd.Request.Difficulty;
        if (cmd.Request.Description != null) existing.Description = cmd.Request.Description;
        if (cmd.Request.Instructions != null) existing.Instructions = cmd.Request.Instructions;
        if (cmd.Request.ImageUrl != null) existing.ImageUrl = cmd.Request.ImageUrl;
        if (cmd.Request.VideoUrl != null) existing.VideoUrl = cmd.Request.VideoUrl;

        var updated = await exercises.UpdateAsync(cmd.Id, existing)
            ?? throw new KeyNotFoundException($"Exercise {cmd.Id} not found");
        return mapper.Map<ExerciseDto>(updated);
    }
}
