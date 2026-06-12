using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class SessionSetDocument
{
    [BsonElement("setNumber")] public int SetNumber { get; set; }
    [BsonElement("reps")] public double? Reps { get; set; }
    [BsonElement("weightKg")] public double? WeightKg { get; set; }
    [BsonElement("durationSec")] public double? DurationSec { get; set; }
    [BsonElement("distanceM")] public double? DistanceM { get; set; }
    [BsonElement("rpe")] public int? Rpe { get; set; }
    [BsonElement("completed")] public bool Completed { get; set; } = true;
}

[BsonIgnoreExtraElements]
public class PerformedExerciseDocument
{
    [BsonElement("exerciseId"), BsonRepresentation(BsonType.ObjectId)] public string ExerciseId { get; set; } = default!;
    [BsonElement("nameSnapshot")] public string? NameSnapshot { get; set; }
    [BsonElement("type")] public string? Type { get; set; }
    [BsonElement("sets")] public List<SessionSetDocument> Sets { get; set; } = [];
}

[BsonIgnoreExtraElements]
public class WorkoutSessionDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("planId"), BsonRepresentation(BsonType.ObjectId)] public string? PlanId { get; set; }
    [BsonElement("planDayNumber")] public int? PlanDayNumber { get; set; }
    [BsonElement("startedAt")] public DateTime StartedAt { get; set; }
    [BsonElement("endedAt")] public DateTime? EndedAt { get; set; }
    [BsonElement("exercises")] public List<PerformedExerciseDocument> Exercises { get; set; } = [];
    [BsonElement("notes")] public string? Notes { get; set; }
    [BsonElement("feeling")] public int? Feeling { get; set; }
    [BsonElement("totalVolumeKg")] public double TotalVolumeKg { get; set; }
    [BsonElement("completedSets")] public int CompletedSets { get; set; }
    [BsonElement("durationSec")] public int DurationSec { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
}
