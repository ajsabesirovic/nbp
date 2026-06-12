using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IExerciseRepository
{
    Task<(List<Exercise> Items, long Total)> GetAllAsync(string? search, string? type, string? muscle, int page, int limit);
    Task<Exercise?> GetByIdAsync(string id);
    Task<Exercise> CreateAsync(Exercise exercise);
    Task<Exercise?> UpdateAsync(string id, Exercise exercise);
    Task<bool> DeleteAsync(string id);
}
