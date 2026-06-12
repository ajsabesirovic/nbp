using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Profile.Commands;

public record SetActivePlanCommand(string UserId, string? PlanId) : IRequest<MeResponse>;
