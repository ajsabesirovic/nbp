using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class GetPlanAssignedClientsQueryHandler(
    IPlanAssignmentRepository assignments,
    IUserRepository users)
    : IRequestHandler<GetPlanAssignedClientsQuery, List<PlanAssignedClientDto>>
{
    public async Task<List<PlanAssignedClientDto>> Handle(GetPlanAssignedClientsQuery q, CancellationToken ct)
    {
        var rows = await assignments.GetActiveAssignmentsForPlanByTrainerAsync(q.PlanId, q.TrainerUserId);
        var result = new List<PlanAssignedClientDto>();
        foreach (var a in rows)
        {
            var u = await users.GetByIdAsync(a.UserId);
            if (u != null)
                result.Add(new PlanAssignedClientDto(u.Id, u.Name, u.Email, a.AssignedAt));
        }
        return result;
    }
}
