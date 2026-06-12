using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IWorkoutPlanRepository
{
    Task<(List<WorkoutPlan> Items, long Total)> GetAllAsync(string? userId, string? visibility, bool mine, bool assignedToMe, List<string>? assignedPlanIds, int page, int limit);
    Task<WorkoutPlan?> GetByIdAsync(string id);
    Task<WorkoutPlan> CreateAsync(WorkoutPlan plan);
    Task<WorkoutPlan?> UpdateAsync(string id, WorkoutPlan plan);
    Task<bool> DeleteAsync(string id);
    Task<(List<WorkoutPlan> Items, long Total)> ListPublicByStatusAsync(string? status, int page, int limit);
    Task<(List<WorkoutPlan> Items, long Total)> AdminListAsync(List<string>? authorIds, string? visibility, int page, int limit);
    Task<long> CountAsync();
    Task<long> CountByVisibilityAndStatusAsync(string visibility, string status);
}
