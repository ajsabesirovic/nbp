using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Plans.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Plans.Handlers;

public class GetPlansQueryHandler(
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    IMapper mapper)
    : IRequestHandler<GetPlansQuery, PagedResult<PlanDto>>
{
    public async Task<PagedResult<PlanDto>> Handle(GetPlansQuery q, CancellationToken ct)
    {

        List<string>? assignedPlanIds = null;
        var needsAssigned = !q.Mine
            && (q.AssignedToMe || string.IsNullOrEmpty(q.Visibility))
            && !string.IsNullOrEmpty(q.UserId);
        if (needsAssigned)
            assignedPlanIds = await assignments.GetActivePlanIdsForUserAsync(q.UserId);

        var (items, total) = await plans.GetAllAsync(
            q.UserId, q.Visibility, q.Mine, q.AssignedToMe, assignedPlanIds, q.Page, q.Limit);
        return new PagedResult<PlanDto>(mapper.Map<List<PlanDto>>(items), total, q.Page, q.Limit);
    }
}
