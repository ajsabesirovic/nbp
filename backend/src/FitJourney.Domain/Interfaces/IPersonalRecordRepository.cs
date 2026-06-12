using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IPersonalRecordRepository
{
    Task<List<PersonalRecord>> GetByUserAsync(string userId);
    Task<PersonalRecord?> GetByUserAndExerciseAsync(string userId, string exerciseId, string type);
    Task<PersonalRecord> UpsertAsync(PersonalRecord pr);
}
