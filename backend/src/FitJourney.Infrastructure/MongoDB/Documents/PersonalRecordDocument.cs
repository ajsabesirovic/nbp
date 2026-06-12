using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class PersonalRecordDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("exerciseId"), BsonRepresentation(BsonType.ObjectId)] public string ExerciseId { get; set; } = default!;
    [BsonElement("exerciseName")] public string? ExerciseName { get; set; }

    [BsonElement("weightKg")] public double WeightKg { get; set; }
    [BsonElement("reps")] public int Reps { get; set; }
    [BsonElement("oneRepMax")] public double OneRepMax { get; set; }

    [BsonElement("type")] public string? Type { get; set; }
    [BsonElement("value")] public double? Value { get; set; }
    [BsonElement("sessionId"), BsonRepresentation(BsonType.ObjectId)] public string? SessionId { get; set; }
    [BsonElement("achievedAt")] public DateTime AchievedAt { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
}
