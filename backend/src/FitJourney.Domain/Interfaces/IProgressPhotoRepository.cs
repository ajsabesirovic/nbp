using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IProgressPhotoRepository
{
    Task<List<ProgressPhoto>> GetByUserAsync(string userId, int limit);
    Task<ProgressPhoto> CreateAsync(ProgressPhoto p);
    Task<bool> DeleteAsync(string id, string userId);
}
