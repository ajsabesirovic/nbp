using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class PlanAssignmentRepository(MongoDbContext db, IMapper mapper) : IPlanAssignmentRepository
{
    private static FilterDefinition<PlanAssignmentDocument> Active =>
        Builders<PlanAssignmentDocument>.Filter.Eq(a => a.Status, "active");

    public async Task<PlanAssignment> AssignAsync(string planId, string userId, string assignedBy)
    {
        var now = DateTime.UtcNow;
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.PlanId, planId), fb.Eq(a => a.UserId, userId));

        var update = Builders<PlanAssignmentDocument>.Update
            .Set(a => a.PlanId, planId)
            .Set(a => a.UserId, userId)
            .Set(a => a.AssignedBy, assignedBy)
            .Set(a => a.Status, "active")
            .Set(a => a.UpdatedAt, now)
            .SetOnInsert(a => a.AssignedAt, now);
        var opts = new FindOneAndUpdateOptions<PlanAssignmentDocument>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After,
        };
        var doc = await db.PlanAssignments.FindOneAndUpdateAsync(filter, update, opts);
        return mapper.Map<PlanAssignment>(doc);
    }

    public async Task<PlanAssignment?> GetAssignmentAsync(string planId, string userId)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.PlanId, planId), fb.Eq(a => a.UserId, userId), Active);
        var doc = await db.PlanAssignments.Find(filter).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<PlanAssignment>(doc);
    }

    public async Task<bool> UnassignAsync(string planId, string userId, string assignedBy)
    {

        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(
            fb.Eq(a => a.PlanId, planId),
            fb.Eq(a => a.UserId, userId),
            fb.Eq(a => a.AssignedBy, assignedBy),
            Active);
        var update = Builders<PlanAssignmentDocument>.Update
            .Set(a => a.Status, "cancelled")
            .Set(a => a.UpdatedAt, DateTime.UtcNow);
        var result = await db.PlanAssignments.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> IsActivelyAssignedAsync(string planId, string userId)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.PlanId, planId), fb.Eq(a => a.UserId, userId), Active);
        return await db.PlanAssignments.Find(filter).AnyAsync();
    }

    public async Task<List<string>> GetActivePlanIdsForUserAsync(string userId)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.UserId, userId), Active);
        var ids = await db.PlanAssignments.Find(filter).Project(a => a.PlanId).ToListAsync();
        return ids.Distinct().ToList();
    }

    public async Task<List<string>> GetActiveUserIdsForPlanAsync(string planId)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.PlanId, planId), Active);
        var ids = await db.PlanAssignments.Find(filter).Project(a => a.UserId).ToListAsync();
        return ids.Distinct().ToList();
    }

    public async Task<List<string>> GetActivePlanIdsByTrainerForUserAsync(string assignedBy, string userId)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.AssignedBy, assignedBy), fb.Eq(a => a.UserId, userId), Active);
        var ids = await db.PlanAssignments.Find(filter).Project(a => a.PlanId).ToListAsync();
        return ids.Distinct().ToList();
    }

    public async Task<List<PlanAssignment>> GetActiveAssignmentsForPlanByTrainerAsync(string planId, string assignedBy)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.PlanId, planId), fb.Eq(a => a.AssignedBy, assignedBy), Active);
        var docs = await db.PlanAssignments.Find(filter).SortBy(a => a.AssignedAt).ToListAsync();
        return mapper.Map<List<PlanAssignment>>(docs);
    }

    public async Task<long> CancelAllByTrainerForUserAsync(string assignedBy, string userId)
    {
        var fb = Builders<PlanAssignmentDocument>.Filter;
        var filter = fb.And(fb.Eq(a => a.AssignedBy, assignedBy), fb.Eq(a => a.UserId, userId), Active);
        var update = Builders<PlanAssignmentDocument>.Update
            .Set(a => a.Status, "cancelled")
            .Set(a => a.UpdatedAt, DateTime.UtcNow);
        var result = await db.PlanAssignments.UpdateManyAsync(filter, update);
        return result.ModifiedCount;
    }
}
