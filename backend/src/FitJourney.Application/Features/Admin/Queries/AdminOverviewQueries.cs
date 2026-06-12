using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Admin.Queries;

public record GetAdminStatsQuery() : IRequest<AdminStatsDto>;

public record GetAdminTrainersQuery() : IRequest<List<AdminTrainerDto>>;

public record AdminListPlansQuery(string? AuthorRole, string? Visibility, int Page, int Limit)
    : IRequest<PagedResult<PlanDto>>;
