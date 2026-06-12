using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class PersonalRecordRepository(MongoDbContext db, IMapper mapper) : IPersonalRecordRepository
{
    public async Task<List<PersonalRecord>> GetByUserAsync(string userId)
    {
        var docs = await db.PersonalRecords.Find(pr => pr.UserId == userId).ToListAsync();

        var missingIds = docs.Where(d => string.IsNullOrEmpty(d.ExerciseName))
                             .Select(d => d.ExerciseId).Distinct().ToList();
        Dictionary<string, string> nameMap = [];
        if (missingIds.Count > 0)
        {
            var oids = missingIds.Select(id => new BsonObjectId(ObjectId.Parse(id))).ToList();
            var exercises = await db.Exercises
                .Find(Builders<ExerciseDocument>.Filter.In("_id", oids))
                .Project(Builders<ExerciseDocument>.Projection.Include(e => e.Name))
                .As<ExerciseDocument>()
                .ToListAsync();
            nameMap = exercises.ToDictionary(e => e.Id, e => e.Name);
        }

        return docs.Select(doc =>
        {
            var pr = mapper.Map<PersonalRecord>(doc);
            if (string.IsNullOrEmpty(pr.ExerciseName) && nameMap.TryGetValue(doc.ExerciseId, out var name))
                pr.ExerciseName = name;

            if (string.IsNullOrEmpty(pr.Type)) pr.Type = doc.Type ?? "1rm";
            if (doc.OneRepMax == 0 && doc.Value.HasValue) pr.OneRepMax = doc.Value.Value;
            return pr;
        }).ToList();
    }

    public async Task<PersonalRecord?> GetByUserAndExerciseAsync(string userId, string exerciseId, string type)
    {
        var doc = await db.PersonalRecords
            .Find(pr => pr.UserId == userId && pr.ExerciseId == exerciseId && pr.Type == type)
            .FirstOrDefaultAsync();
        if (doc == null) return null;
        var pr = mapper.Map<PersonalRecord>(doc);
        if (string.IsNullOrEmpty(pr.Type)) pr.Type = doc.Type ?? "1rm";
        if (doc.OneRepMax == 0 && doc.Value.HasValue) pr.OneRepMax = doc.Value.Value;
        return pr;
    }

    public async Task<PersonalRecord> UpsertAsync(PersonalRecord pr)
    {
        var doc = mapper.Map<PersonalRecordDocument>(pr);
        if (string.IsNullOrEmpty(doc.Id)) doc.Id = ObjectId.GenerateNewId().ToString();
        var fb = Builders<PersonalRecordDocument>.Filter;
        var filter = fb.And(
            fb.Eq(x => x.UserId, pr.UserId),
            fb.Eq(x => x.ExerciseId, pr.ExerciseId),
            fb.Eq(x => x.Type, pr.Type)
        );
        var options = new ReplaceOptions { IsUpsert = true };
        await db.PersonalRecords.ReplaceOneAsync(filter, doc, options);
        var updated = await db.PersonalRecords.Find(filter).FirstOrDefaultAsync();
        return mapper.Map<PersonalRecord>(updated!);
    }
}
