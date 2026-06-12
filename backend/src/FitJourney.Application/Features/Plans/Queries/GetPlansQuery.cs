using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Plans.Queries;

public record GetPlansQuery(string UserId, string? Visibility, bool Mine, bool AssignedToMe, int Page, int Limit) : IRequest<PagedResult<PlanDto>>;
