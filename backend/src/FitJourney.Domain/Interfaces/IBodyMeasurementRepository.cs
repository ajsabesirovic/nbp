using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IBodyMeasurementRepository
{
    Task<List<BodyMeasurement>> GetByUserAsync(string userId, int limit);
    Task<BodyMeasurement?> GetByIdAsync(string id);
    Task<BodyMeasurement> CreateAsync(BodyMeasurement m);
    Task<bool> DeleteAsync(string id, string userId);
}
