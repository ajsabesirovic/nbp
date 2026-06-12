using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Admin.Handlers;

public class RecentLogsQueryHandler(IRequestLogRepository logs, IMapper mapper)
    : IRequestHandler<RecentLogsQuery, PagedResult<RequestLogDto>>
{
    public async Task<PagedResult<RequestLogDto>> Handle(RecentLogsQuery q, CancellationToken ct)
    {
        var (items, total) = await logs.RecentAsync(q.Page, q.Limit);
        return new PagedResult<RequestLogDto>(mapper.Map<List<RequestLogDto>>(items), total, q.Page, q.Limit);
    }
}

public class SlowLogsQueryHandler(IRequestLogRepository logs, IMapper mapper)
    : IRequestHandler<SlowLogsQuery, PagedResult<RequestLogDto>>
{
    public async Task<PagedResult<RequestLogDto>> Handle(SlowLogsQuery q, CancellationToken ct)
    {
        var (items, total) = await logs.SlowAsync(q.ThresholdMs, q.Page, q.Limit);
        return new PagedResult<RequestLogDto>(mapper.Map<List<RequestLogDto>>(items), total, q.Page, q.Limit);
    }
}
