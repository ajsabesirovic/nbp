using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Admin.Commands;

public record ModeratePlanCommand(string PlanId, string Status) : IRequest<PlanDto>;
