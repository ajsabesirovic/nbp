using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class TrainerProfileRepository(MongoDbContext db, IMapper mapper) : ITrainerProfileRepository
{
    public async Task<TrainerProfile?> GetByUserIdAsync(string userId)
    {
        var doc = await db.TrainerProfiles.Find(p => p.UserId == userId).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<TrainerProfile>(doc);
    }

    public async Task<TrainerProfile> UpsertAsync(TrainerProfile profile)
    {
        var existing = await db.TrainerProfiles.Find(p => p.UserId == profile.UserId).FirstOrDefaultAsync();
        var now = DateTime.UtcNow;

        if (existing == null)
        {
            var doc = mapper.Map<TrainerProfileDocument>(profile);
            doc.Id = string.Empty;
            doc.CreatedAt = now;
            doc.UpdatedAt = now;
            await db.TrainerProfiles.InsertOneAsync(doc);
            return mapper.Map<TrainerProfile>(doc);
        }

        var doc2 = mapper.Map<TrainerProfileDocument>(profile);
        doc2.Id = existing.Id;
        doc2.CreatedAt = existing.CreatedAt;
        doc2.UpdatedAt = now;
        await db.TrainerProfiles.ReplaceOneAsync(p => p.Id == existing.Id, doc2);
        return mapper.Map<TrainerProfile>(doc2);
    }

    public async Task<bool> AddClientAsync(string trainerUserId, string clientUserId)
    {
        var update = Builders<TrainerProfileDocument>.Update
            .AddToSet(p => p.ClientIds, clientUserId)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);
        var result = await db.TrainerProfiles.UpdateOneAsync(
            p => p.UserId == trainerUserId, update, new UpdateOptions { IsUpsert = true });
        return result.IsAcknowledged;
    }

    public async Task<bool> RemoveClientAsync(string trainerUserId, string clientUserId)
    {
        var update = Builders<TrainerProfileDocument>.Update
            .Pull(p => p.ClientIds, clientUserId)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);
        var result = await db.TrainerProfiles.UpdateOneAsync(p => p.UserId == trainerUserId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<List<TrainerProfile>> GetTrainersForClientAsync(string clientUserId)
    {
        var docs = await db.TrainerProfiles.Find(p => p.ClientIds.Contains(clientUserId)).ToListAsync();
        return mapper.Map<List<TrainerProfile>>(docs);
    }
}
