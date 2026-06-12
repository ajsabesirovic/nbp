using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Plans.Queries;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Plans.Handlers;

public class GetPlanByIdQueryHandler(
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    IMapper mapper)
    : IRequestHandler<GetPlanByIdQuery, PlanDto>
{
    public async Task<PlanDto> Handle(GetPlanByIdQuery q, CancellationToken ct)
    {
        var plan = await plans.GetByIdAsync(q.Id)
            ?? throw new KeyNotFoundException($"Plan {q.Id} not found");

        var allowed = q.IsAdmin
            || plan.Visibility == Visibility.@public
            || plan.AuthorId == q.RequesterId
            || await assignments.IsActivelyAssignedAsync(plan.Id, q.RequesterId);
        if (!allowed)
            throw new ForbiddenException("You don't have access to this plan.");

        return mapper.Map<PlanDto>(plan);
    }
}
