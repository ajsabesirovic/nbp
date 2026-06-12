using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IPlanAssignmentRepository
{
    Task<PlanAssignment> AssignAsync(string planId, string userId, string assignedBy);
    Task<bool> UnassignAsync(string planId, string userId, string assignedBy);
    Task<bool> IsActivelyAssignedAsync(string planId, string userId);

    Task<PlanAssignment?> GetAssignmentAsync(string planId, string userId);
    Task<List<string>> GetActivePlanIdsForUserAsync(string userId);
    Task<List<string>> GetActiveUserIdsForPlanAsync(string planId);
    Task<List<string>> GetActivePlanIdsByTrainerForUserAsync(string assignedBy, string userId);
    Task<List<PlanAssignment>> GetActiveAssignmentsForPlanByTrainerAsync(string planId, string assignedBy);
    Task<long> CancelAllByTrainerForUserAsync(string assignedBy, string userId);
}
