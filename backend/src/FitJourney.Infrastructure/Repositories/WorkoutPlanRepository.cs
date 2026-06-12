using AutoMapper;
using MongoDB.Driver;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
using FitJourney.Infrastructure.MongoDB;
using FitJourney.Infrastructure.MongoDB.Documents;
namespace FitJourney.Infrastructure.Repositories;

public class WorkoutPlanRepository(MongoDbContext db, IMapper mapper) : IWorkoutPlanRepository
{
    public async Task<(List<WorkoutPlan> Items, long Total)> GetAllAsync(string? userId, string? visibility, bool mine, bool assignedToMe, List<string>? assignedPlanIds, int page, int limit)
    {
        var fb = Builders<WorkoutPlanDocument>.Filter;
        FilterDefinition<WorkoutPlanDocument> filter;

        if (mine)
        {
            if (string.IsNullOrEmpty(userId)) return ([], 0);
            filter = fb.Eq(p => p.AuthorId, userId);
        }
        else if (assignedToMe)
        {

            var ids = assignedPlanIds ?? [];
            if (ids.Count == 0) return ([], 0);
            filter = fb.In(p => p.Id, ids);
        }
        else if (!string.IsNullOrEmpty(visibility))
        {
            filter = fb.Eq(p => p.Visibility, visibility);
        }
        else if (!string.IsNullOrEmpty(userId))
        {

            var ors = new List<FilterDefinition<WorkoutPlanDocument>>
            {
                fb.Eq(p => p.AuthorId, userId),
                fb.Eq(p => p.Visibility, "public"),
            };
            if (assignedPlanIds is { Count: > 0 })
                ors.Add(fb.In(p => p.Id, assignedPlanIds));
            filter = fb.Or(ors);
        }
        else
        {
            filter = fb.Eq(p => p.Visibility, "public");
        }

        var total = await db.Plans.CountDocumentsAsync(filter);
        var docs = await db.Plans
            .Find(filter)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();

        return (mapper.Map<List<WorkoutPlan>>(docs), total);
    }

    public async Task<WorkoutPlan?> GetByIdAsync(string id)
    {
        var doc = await db.Plans.Find(p => p.Id == id).FirstOrDefaultAsync();
        return doc == null ? null : mapper.Map<WorkoutPlan>(doc);
    }

    public async Task<WorkoutPlan> CreateAsync(WorkoutPlan plan)
    {
        var doc = mapper.Map<WorkoutPlanDocument>(plan);
        doc.Id = string.Empty;
        await db.Plans.InsertOneAsync(doc);
        return mapper.Map<WorkoutPlan>(doc);
    }

    public async Task<WorkoutPlan?> UpdateAsync(string id, WorkoutPlan plan)
    {
        var doc = mapper.Map<WorkoutPlanDocument>(plan);
        doc.Id = id;
        var result = await db.Plans.ReplaceOneAsync(p => p.Id == id, doc);
        if (result.MatchedCount == 0) return null;
        return mapper.Map<WorkoutPlan>(doc);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await db.Plans.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<(List<WorkoutPlan> Items, long Total)> AdminListAsync(List<string>? authorIds, string? visibility, int page, int limit)
    {
        var fb = Builders<WorkoutPlanDocument>.Filter;
        var clauses = new List<FilterDefinition<WorkoutPlanDocument>>();
        if (authorIds != null)
        {

            if (authorIds.Count == 0) return ([], 0);
            clauses.Add(fb.In(p => p.AuthorId, authorIds));
        }
        if (!string.IsNullOrWhiteSpace(visibility))
            clauses.Add(fb.Eq(p => p.Visibility, visibility));

        var filter = clauses.Count == 0 ? fb.Empty : fb.And(clauses);
        var total = await db.Plans.CountDocumentsAsync(filter);
        var docs = await db.Plans.Find(filter)
            .SortByDescending(p => p.UpdatedAt)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
        return (mapper.Map<List<WorkoutPlan>>(docs), total);
    }

    public async Task<long> CountAsync()
    {
        return await db.Plans.CountDocumentsAsync(Builders<WorkoutPlanDocument>.Filter.Empty);
    }

    public async Task<long> CountByVisibilityAndStatusAsync(string visibility, string status)
    {
        var fb = Builders<WorkoutPlanDocument>.Filter;
        return await db.Plans.CountDocumentsAsync(
            fb.And(fb.Eq(p => p.Visibility, visibility), fb.Eq(p => p.Status, status)));
    }

    public async Task<(List<WorkoutPlan> Items, long Total)> ListPublicByStatusAsync(string? status, int page, int limit)
    {
        var fb = Builders<WorkoutPlanDocument>.Filter;
        var clauses = new List<FilterDefinition<WorkoutPlanDocument>>
        {
            fb.Eq(p => p.Visibility, "public"),
        };
        if (!string.IsNullOrWhiteSpace(status))
            clauses.Add(fb.Eq(p => p.Status, status));
        var filter = fb.And(clauses);

        var total = await db.Plans.CountDocumentsAsync(filter);
        var docs = await db.Plans.Find(filter)
            .SortByDescending(p => p.UpdatedAt)
            .Skip((page - 1) * limit)
            .Limit(limit)
            .ToListAsync();
        return (mapper.Map<List<WorkoutPlan>>(docs), total);
    }
}
