using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Commands;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Admin.Handlers;

public class ModeratePlanCommandHandler(IWorkoutPlanRepository plans, IMapper mapper)
    : IRequestHandler<ModeratePlanCommand, PlanDto>
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "draft", "published", "archived"
    };

    public async Task<PlanDto> Handle(ModeratePlanCommand cmd, CancellationToken ct)
    {
        if (!Allowed.Contains(cmd.Status))
            throw new ArgumentException($"Invalid plan status '{cmd.Status}'. Allowed: draft, published, archived.");

        var plan = await plans.GetByIdAsync(cmd.PlanId)
            ?? throw new KeyNotFoundException($"Plan {cmd.PlanId} not found");

        plan.Status = cmd.Status.ToLowerInvariant();
        plan.UpdatedAt = DateTime.UtcNow;
        var updated = await plans.UpdateAsync(cmd.PlanId, plan)
            ?? throw new KeyNotFoundException($"Plan {cmd.PlanId} not found");
        return mapper.Map<PlanDto>(updated);
    }
}
