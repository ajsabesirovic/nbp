using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Plans.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Plans.Handlers;

public class UpdatePlanCommandHandler(IWorkoutPlanRepository plans, IMapper mapper)
    : IRequestHandler<UpdatePlanCommand, PlanDto>
{
    public async Task<PlanDto> Handle(UpdatePlanCommand cmd, CancellationToken ct)
    {
        var existing = await plans.GetByIdAsync(cmd.Id)
            ?? throw new KeyNotFoundException($"Plan {cmd.Id} not found");

        if (existing.AuthorId != cmd.UserId)
            throw new ForbiddenException("You are not the author of this plan");

        if (cmd.Request.Name != null) existing.Name = cmd.Request.Name;
        if (cmd.Request.Description != null) existing.Description = cmd.Request.Description;
        if (cmd.Request.DurationWeeks.HasValue) existing.DurationWeeks = cmd.Request.DurationWeeks.Value;
        if (cmd.Request.Level != null) existing.Level = Enum.Parse<PlanLevel>(cmd.Request.Level, ignoreCase: true);
        if (cmd.Request.Goal != null) existing.Goal = Enum.Parse<PlanGoal>(cmd.Request.Goal, ignoreCase: true);
        if (cmd.Request.DaysPerWeek.HasValue) existing.DaysPerWeek = cmd.Request.DaysPerWeek.Value;
        if (cmd.Request.Visibility != null
            && Enum.TryParse<Visibility>(cmd.Request.Visibility, ignoreCase: true, out var vis))
        {
            existing.Visibility = vis;
        }
        if (cmd.Request.Status != null) existing.Status = cmd.Request.Status;
        if (cmd.Request.Days != null) existing.Days = mapper.Map<List<WorkoutPlanDay>>(cmd.Request.Days);
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await plans.UpdateAsync(cmd.Id, existing)
            ?? throw new KeyNotFoundException($"Plan {cmd.Id} not found");
        return mapper.Map<PlanDto>(updated);
    }
}
