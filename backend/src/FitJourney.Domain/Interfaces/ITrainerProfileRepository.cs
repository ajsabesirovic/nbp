using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface ITrainerProfileRepository
{
    Task<TrainerProfile?> GetByUserIdAsync(string userId);
    Task<TrainerProfile> UpsertAsync(TrainerProfile profile);
    Task<bool> AddClientAsync(string trainerUserId, string clientUserId);
    Task<bool> RemoveClientAsync(string trainerUserId, string clientUserId);
    Task<List<TrainerProfile>> GetTrainersForClientAsync(string clientUserId);
}
