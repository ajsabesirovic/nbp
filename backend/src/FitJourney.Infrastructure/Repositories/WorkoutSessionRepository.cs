using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class WorkoutSessionRepository(MongoDbContext db, IMapper mapper) : IWorkoutSessionRepository
{
    public async Task<(List<WorkoutSession> Items, long Total)> GetByUserAsync(string userId, int page, int limit)
    {
        var filter = Builders<WorkoutSessionDocument>.Filter.Eq(s => s.UserId, userId);
        var total = await db.Sessions.CountDocumentsAsync(filter);
        var docs = await db.Sessions
            .Find(filter)
            .SortByDescending(s => s.StartedAt)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
        return (mapper.Map<List<WorkoutSession>>(docs), total);
    }

    public async Task<WorkoutSession?> GetByIdAsync(string id)
    {
        var doc = await db.Sessions.Find(s => s.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<WorkoutSession>(doc);
    }

    public async Task<WorkoutSession> CreateAsync(WorkoutSession session)
    {
        var doc = mapper.Map<WorkoutSessionDocument>(session);
        doc.Id = string.Empty;
        await db.Sessions.InsertOneAsync(doc);
        return mapper.Map<WorkoutSession>(doc);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await db.Sessions.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<WorkoutSession>> GetForStreakAsync(string userId)
    {
        var filter = Builders<WorkoutSessionDocument>.Filter.Eq(s => s.UserId, userId);
        var docs = await db.Sessions
            .Find(filter)
            .Project(Builders<WorkoutSessionDocument>.Projection.Include(s => s.StartedAt).Include(s => s.UserId))
            .As<WorkoutSessionDocument>()
            .ToListAsync();
        return mapper.Map<List<WorkoutSession>>(docs);
    }

    public async Task<List<WeeklyVolumeResult>> GetWeeklyVolumeAsync(string userId, DateTime from)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "userId", new BsonObjectId(ObjectId.Parse(userId)) },
                { "startedAt", new BsonDocument("$gte", from) }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument
                    {
                        { "year", new BsonDocument("$isoWeekYear", "$startedAt") },
                        { "week", new BsonDocument("$isoWeek", "$startedAt") }
                    }
                },
                { "totalVolumeKg", new BsonDocument("$sum", "$totalVolumeKg") },
                { "sessionCount", new BsonDocument("$sum", 1) },
                { "totalSets", new BsonDocument("$sum", "$completedSets") }
            }),
            new BsonDocument("$sort", new BsonDocument
            {
                { "_id.year", 1 },
                { "_id.week", 1 }
            })
        };

        var result = await db.Sessions.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return result.Select(doc => new WeeklyVolumeResult(
            doc["_id"]["year"].ToInt32(),
            doc["_id"]["week"].ToInt32(),
            doc["totalVolumeKg"].ToDouble(),
            doc["sessionCount"].ToInt32(),
            doc["totalSets"].ToInt32()
        )).ToList();
    }

    public async Task<List<MuscleBalanceResult>> GetMuscleBalanceAsync(string userId, DateTime from)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "userId", new BsonObjectId(ObjectId.Parse(userId)) },
                { "startedAt", new BsonDocument("$gte", from) }
            }),
            new BsonDocument("$unwind", "$exercises"),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "exercises" },
                { "localField", "exercises.exerciseId" },
                { "foreignField", "_id" },
                { "as", "exerciseDef" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$exerciseDef" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$exerciseDef.primaryMuscles" },
                { "preserveNullAndEmptyArrays", false }
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "completedSets", new BsonDocument("$filter", new BsonDocument
                    {
                        { "input", "$exercises.sets" },
                        { "as", "s" },
                        { "cond", "$$s.completed" }
                    })
                }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$exerciseDef.primaryMuscles" },
                { "sets", new BsonDocument("$sum", new BsonDocument("$size", "$completedSets")) },
                { "volumeKg", new BsonDocument("$sum", new BsonDocument("$reduce", new BsonDocument
                    {
                        { "input", "$completedSets" },
                        { "initialValue", 0.0 },
                        { "in", new BsonDocument("$add", new BsonArray
                            {
                                "$$value",
                                new BsonDocument("$multiply", new BsonArray
                                {
                                    new BsonDocument("$ifNull", new BsonArray { "$$this.weightKg", 0.0 }),
                                    new BsonDocument("$ifNull", new BsonArray { "$$this.reps", 0.0 })
                                })
                            })
                        }
                    }))
                }
            }),
            new BsonDocument("$sort", new BsonDocument("volumeKg", -1))
        };

        var result = await db.Sessions.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return result.Select(doc => new MuscleBalanceResult(
            doc["_id"].AsString,
            doc["sets"].AsInt32,
            doc["volumeKg"].ToDouble()
        )).ToList();
    }

    public async Task<List<ProgressionResult>> GetProgressionAsync(string userId, string exerciseId)
    {
        var userOid = new BsonObjectId(ObjectId.Parse(userId));
        var exOid = new BsonObjectId(ObjectId.Parse(exerciseId));
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "userId", userOid },
                { "exercises.exerciseId", exOid }
            }),
            new BsonDocument("$unwind", "$exercises"),
            new BsonDocument("$match", new BsonDocument
            {
                { "exercises.exerciseId", exOid }
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "completedSets", new BsonDocument("$filter", new BsonDocument
                    {
                        { "input", "$exercises.sets" },
                        { "as", "s" },
                        { "cond", new BsonDocument("$and", new BsonArray
                            {
                                "$$s.completed",
                                new BsonDocument("$gt", new BsonArray { new BsonDocument("$ifNull", new BsonArray { "$$s.weightKg", BsonNull.Value }), BsonNull.Value }),
                                new BsonDocument("$gt", new BsonArray { new BsonDocument("$ifNull", new BsonArray { "$$s.reps", BsonNull.Value }), BsonNull.Value })
                            })
                        }
                    })
                }
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "maxWeight", new BsonDocument("$max", "$completedSets.weightKg") }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument("$dateToString", new BsonDocument
                    {
                        { "format", "%Y-%m-%d" },
                        { "date", "$startedAt" }
                    })
                },
                { "maxWeightKg", new BsonDocument("$max", "$maxWeight") },
                { "maxReps", new BsonDocument("$max", new BsonDocument("$max", "$completedSets.reps")) },
                { "totalSets", new BsonDocument("$sum", new BsonDocument("$size", "$completedSets")) },
                { "totalVolumeKg", new BsonDocument("$sum", new BsonDocument("$reduce", new BsonDocument
                    {
                        { "input", "$completedSets" },
                        { "initialValue", 0.0 },
                        { "in", new BsonDocument("$add", new BsonArray
                            {
                                "$$value",
                                new BsonDocument("$multiply", new BsonArray
                                {
                                    new BsonDocument("$ifNull", new BsonArray { "$$this.weightKg", 0.0 }),
                                    new BsonDocument("$ifNull", new BsonArray { "$$this.reps", 0.0 })
                                })
                            })
                        }
                    }))
                }
            }),
            new BsonDocument("$sort", new BsonDocument("_id", 1))
        };

        var result = await db.Sessions.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return result.Select(doc =>
        {
            double maxWeight = doc.Contains("maxWeightKg") && !doc["maxWeightKg"].IsBsonNull ? doc["maxWeightKg"].ToDouble() : 0;
            double maxReps = doc.Contains("maxReps") && !doc["maxReps"].IsBsonNull ? doc["maxReps"].ToDouble() : 0;
            double oneRepMax = maxReps > 0 ? maxWeight * (1 + maxReps / 30.0) : maxWeight;
            return new ProgressionResult(
                DateTime.Parse(doc["_id"].AsString),
                maxWeight,
                oneRepMax,
                doc["totalSets"].AsInt32,
                doc["totalVolumeKg"].ToDouble()
            );
        }).ToList();
    }

    public async Task<List<WorkoutSession>> GetRecentAsync(string userId, int count)
    {
        var filter = Builders<WorkoutSessionDocument>.Filter.Eq(s => s.UserId, userId);
        var docs = await db.Sessions
            .Find(filter)
            .SortByDescending(s => s.StartedAt)
            .Limit(count)
            .ToListAsync();
        return mapper.Map<List<WorkoutSession>>(docs);
    }

    public async Task<long> CountByUserAndPlanAsync(string userId, string planId)
    {
        var fb = Builders<WorkoutSessionDocument>.Filter;
        var filter = fb.And(fb.Eq(s => s.UserId, userId), fb.Eq(s => s.PlanId, planId));
        return await db.Sessions.CountDocumentsAsync(filter);
    }

    public async Task<long> CountAsync()
    {
        return await db.Sessions.CountDocumentsAsync(Builders<WorkoutSessionDocument>.Filter.Empty);
    }

    public async Task<long> CountSinceAsync(DateTime since)
    {
        return await db.Sessions.CountDocumentsAsync(
            Builders<WorkoutSessionDocument>.Filter.Gte(s => s.StartedAt, since));
    }

    public async Task<long> CountActiveUsersSinceAsync(DateTime since)
    {
        var distinct = await db.Sessions
            .Distinct(s => s.UserId, Builders<WorkoutSessionDocument>.Filter.Gte(s => s.StartedAt, since))
            .ToListAsync();
        return distinct.Count;
    }
}
