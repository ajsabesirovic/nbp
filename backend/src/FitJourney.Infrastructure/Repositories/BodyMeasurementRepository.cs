using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class BodyMeasurementRepository(MongoDbContext db, IMapper mapper) : IBodyMeasurementRepository
{
    public async Task<List<BodyMeasurement>> GetByUserAsync(string userId, int limit)
    {
        var docs = await db.BodyMeasurements
            .Find(m => m.UserId == userId)
            .SortByDescending(m => m.Date)
            .Limit(limit)
            .ToListAsync();
        return mapper.Map<List<BodyMeasurement>>(docs);
    }

    public async Task<BodyMeasurement?> GetByIdAsync(string id)
    {
        var doc = await db.BodyMeasurements.Find(m => m.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<BodyMeasurement>(doc);
    }

    public async Task<BodyMeasurement> CreateAsync(BodyMeasurement m)
    {
        var doc = mapper.Map<BodyMeasurementDocument>(m);
        doc.Id = string.Empty;
        await db.BodyMeasurements.InsertOneAsync(doc);
        return mapper.Map<BodyMeasurement>(doc);
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var result = await db.BodyMeasurements.DeleteOneAsync(m => m.Id == id && m.UserId == userId);
        return result.DeletedCount > 0;
    }
}
