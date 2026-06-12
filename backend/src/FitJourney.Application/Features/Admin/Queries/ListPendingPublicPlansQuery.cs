using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Admin.Queries;

public record ListPendingPublicPlansQuery(string? Status, int Page, int Limit) : IRequest<PagedResult<PlanDto>>;
