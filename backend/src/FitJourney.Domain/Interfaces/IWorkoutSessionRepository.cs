using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IWorkoutSessionRepository
{
    Task<(List<WorkoutSession> Items, long Total)> GetByUserAsync(string userId, int page, int limit);
    Task<WorkoutSession?> GetByIdAsync(string id);
    Task<WorkoutSession> CreateAsync(WorkoutSession session);
    Task<bool> DeleteAsync(string id);
    Task<List<WorkoutSession>> GetForStreakAsync(string userId);
    Task<List<WeeklyVolumeResult>> GetWeeklyVolumeAsync(string userId, DateTime from);
    Task<List<MuscleBalanceResult>> GetMuscleBalanceAsync(string userId, DateTime from);
    Task<List<ProgressionResult>> GetProgressionAsync(string userId, string exerciseId);
    Task<List<WorkoutSession>> GetRecentAsync(string userId, int count);
    Task<long> CountByUserAndPlanAsync(string userId, string planId);
    Task<long> CountAsync();
    Task<long> CountSinceAsync(DateTime since);
    Task<long> CountActiveUsersSinceAsync(DateTime since);
}

public record WeeklyVolumeResult(int Year, int Week, double TotalVolumeKg, int SessionCount, int TotalSets);
public record MuscleBalanceResult(string Muscle, int Sets, double VolumeKg);
public record ProgressionResult(DateTime Date, double MaxWeightKg, double OneRepMax, int TotalSets, double TotalVolumeKg);
