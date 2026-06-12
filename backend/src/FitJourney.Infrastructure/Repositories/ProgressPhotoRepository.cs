using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class ProgressPhotoRepository(MongoDbContext db, IMapper mapper) : IProgressPhotoRepository
{
    public async Task<List<ProgressPhoto>> GetByUserAsync(string userId, int limit)
    {
        var docs = await db.ProgressPhotos
            .Find(p => p.UserId == userId)
            .SortByDescending(p => p.TakenAt)
            .Limit(limit)
            .ToListAsync();
        return mapper.Map<List<ProgressPhoto>>(docs);
    }

    public async Task<ProgressPhoto> CreateAsync(ProgressPhoto p)
    {
        var doc = mapper.Map<ProgressPhotoDocument>(p);
        doc.Id = string.Empty;
        await db.ProgressPhotos.InsertOneAsync(doc);
        return mapper.Map<ProgressPhoto>(doc);
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var result = await db.ProgressPhotos.DeleteOneAsync(p => p.Id == id && p.UserId == userId);
        return result.DeletedCount > 0;
    }
}
