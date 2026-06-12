using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using FitJourney.Infrastructure.MongoDB.Documents;
using FitJourney.Infrastructure.Settings;
namespace FitJourney.Infrastructure.MongoDB;

public class MongoDbContext
{
    private const string RequestLogsCollection = "requestLogs";
    private const long RequestLogsCappedBytes = 500L * 1024 * 1024;

    private readonly IMongoDatabase _db;

    public MongoDbContext(IOptions<MongoSettings> opts)
    {
        var client = new MongoClient(opts.Value.ConnectionString);
        _db = client.GetDatabase(opts.Value.DatabaseName);
        EnsureRequestLogsCapped();
        EnsureIndexes();
    }

    private void EnsureIndexes()
    {
        TrainerProfiles.Indexes.CreateOne(new CreateIndexModel<TrainerProfileDocument>(
            Builders<TrainerProfileDocument>.IndexKeys.Ascending(t => t.UserId),
            new CreateIndexOptions { Unique = true, Name = "ux_trainerProfiles_userId" }));

        RequestLogs.Indexes.CreateOne(new CreateIndexModel<RequestLogDocument>(
            Builders<RequestLogDocument>.IndexKeys.Descending(r => r.Timestamp),
            new CreateIndexOptions { Name = "ix_requestLogs_ts_desc" }));

        PlanAssignments.Indexes.CreateOne(new CreateIndexModel<PlanAssignmentDocument>(
            Builders<PlanAssignmentDocument>.IndexKeys.Ascending(a => a.PlanId).Ascending(a => a.UserId),
            new CreateIndexOptions { Unique = true, Name = "ux_planAssignments_plan_user" }));
        PlanAssignments.Indexes.CreateOne(new CreateIndexModel<PlanAssignmentDocument>(
            Builders<PlanAssignmentDocument>.IndexKeys.Ascending(a => a.UserId).Ascending(a => a.Status),
            new CreateIndexOptions { Name = "ix_planAssignments_user_status" }));
        PlanAssignments.Indexes.CreateOne(new CreateIndexModel<PlanAssignmentDocument>(
            Builders<PlanAssignmentDocument>.IndexKeys.Ascending(a => a.AssignedBy),
            new CreateIndexOptions { Name = "ix_planAssignments_assignedBy" }));
    }

    private void EnsureRequestLogsCapped()
    {
        var filter = new BsonDocument("name", RequestLogsCollection);
        var existing = _db.ListCollectionNames(new ListCollectionNamesOptions { Filter = filter }).FirstOrDefault();
        if (existing != null) return;
        _db.CreateCollection(RequestLogsCollection, new CreateCollectionOptions
        {
            Capped = true,
            MaxSize = RequestLogsCappedBytes,
        });
    }

    public IMongoCollection<UserDocument> Users => _db.GetCollection<UserDocument>("users");
    public IMongoCollection<ExerciseDocument> Exercises => _db.GetCollection<ExerciseDocument>("exercises");
    public IMongoCollection<WorkoutPlanDocument> Plans => _db.GetCollection<WorkoutPlanDocument>("workoutplans");
    public IMongoCollection<WorkoutSessionDocument> Sessions => _db.GetCollection<WorkoutSessionDocument>("workoutsessions");
    public IMongoCollection<PersonalRecordDocument> PersonalRecords => _db.GetCollection<PersonalRecordDocument>("personalrecords");
    public IMongoCollection<RefreshTokenDocument> RefreshTokens => _db.GetCollection<RefreshTokenDocument>("refreshtokens");
    public IMongoCollection<BodyMeasurementDocument> BodyMeasurements => _db.GetCollection<BodyMeasurementDocument>("bodymeasurements");
    public IMongoCollection<ProgressPhotoDocument> ProgressPhotos => _db.GetCollection<ProgressPhotoDocument>("progressphotos");
    public IMongoCollection<MessageDocument> Messages => _db.GetCollection<MessageDocument>("messages");
    public IMongoCollection<NotificationDocument> Notifications => _db.GetCollection<NotificationDocument>("notifications");
    public IMongoCollection<RequestLogDocument> RequestLogs => _db.GetCollection<RequestLogDocument>(RequestLogsCollection);
    public IMongoCollection<TrainerProfileDocument> TrainerProfiles => _db.GetCollection<TrainerProfileDocument>("trainerProfiles");
    public IMongoCollection<PlanAssignmentDocument> PlanAssignments => _db.GetCollection<PlanAssignmentDocument>("planAssignments");
}
