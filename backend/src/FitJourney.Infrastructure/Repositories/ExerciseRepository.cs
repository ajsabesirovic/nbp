using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class ExerciseRepository(MongoDbContext db, IMapper mapper) : IExerciseRepository
{
    public async Task<(List<Exercise> Items, long Total)> GetAllAsync(string? search, string? type, string? muscle, int page, int limit)
    {
        var filterBuilder = Builders<ExerciseDocument>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(search))
            filter &= filterBuilder.Regex(e => e.Name, new BsonRegularExpression(search, "i"));

        if (!string.IsNullOrWhiteSpace(type))
            filter &= filterBuilder.Eq(e => e.Type, type.ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(muscle))
            filter &= filterBuilder.AnyEq(e => e.PrimaryMuscles, muscle.ToLowerInvariant());

        var total = await db.Exercises.CountDocumentsAsync(filter);
        var docs = await db.Exercises
            .Find(filter)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        return (mapper.Map<List<Exercise>>(docs), total);
    }

    public async Task<Exercise?> GetByIdAsync(string id)
    {
        var doc = await db.Exercises.Find(e => e.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<Exercise>(doc);
    }

    public async Task<Exercise> CreateAsync(Exercise exercise)
    {
        var doc = mapper.Map<ExerciseDocument>(exercise);
        doc.Id = string.Empty;
        await db.Exercises.InsertOneAsync(doc);
        return mapper.Map<Exercise>(doc);
    }

    public async Task<Exercise?> UpdateAsync(string id, Exercise exercise)
    {
        var doc = mapper.Map<ExerciseDocument>(exercise);
        doc.Id = id;
        var result = await db.Exercises.ReplaceOneAsync(e => e.Id == id, doc);
        if (result.MatchedCount == 0) return null;
        return mapper.Map<Exercise>(doc);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await db.Exercises.DeleteOneAsync(e => e.Id == id);
        return result.DeletedCount > 0;
    }
}
