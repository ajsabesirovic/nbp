using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Admin.Handlers;

public class ListPendingPublicPlansQueryHandler(IWorkoutPlanRepository plans, IMapper mapper)
    : IRequestHandler<ListPendingPublicPlansQuery, PagedResult<PlanDto>>
{
    public async Task<PagedResult<PlanDto>> Handle(ListPendingPublicPlansQuery q, CancellationToken ct)
    {
        var (items, total) = await plans.ListPublicByStatusAsync(q.Status, q.Page, q.Limit);
        return new PagedResult<PlanDto>(mapper.Map<List<PlanDto>>(items), total, q.Page, q.Limit);
    }
}
