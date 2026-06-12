using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Plans.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Plans.Handlers;

public class CreatePlanCommandHandler(IWorkoutPlanRepository plans, IMapper mapper)
    : IRequestHandler<CreatePlanCommand, PlanDto>
{
    public async Task<PlanDto> Handle(CreatePlanCommand cmd, CancellationToken ct)
    {
        var visibility = Enum.TryParse<Visibility>(cmd.Request.Visibility, ignoreCase: true, out var vis)
            ? vis : Visibility.@private;

        var entity = new WorkoutPlan
        {
            AuthorId = cmd.AuthorId,
            AuthorName = cmd.AuthorName,
            Name = cmd.Request.Name,
            Description = cmd.Request.Description,
            DurationWeeks = cmd.Request.DurationWeeks,
            Level = Enum.Parse<PlanLevel>(cmd.Request.Level, ignoreCase: true),
            Goal = Enum.Parse<PlanGoal>(cmd.Request.Goal, ignoreCase: true),
            DaysPerWeek = cmd.Request.DaysPerWeek,
            Visibility = visibility,
            Status = cmd.Request.Status,
            Days = mapper.Map<List<WorkoutPlanDay>>(cmd.Request.Days),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var created = await plans.CreateAsync(entity);
        return mapper.Map<PlanDto>(created);
    }
}
