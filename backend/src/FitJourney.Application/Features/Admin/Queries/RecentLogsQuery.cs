using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Admin.Queries;

public record RecentLogsQuery(int Page, int Limit) : IRequest<PagedResult<RequestLogDto>>;
public record SlowLogsQuery(long ThresholdMs, int Page, int Limit) : IRequest<PagedResult<RequestLogDto>>;
