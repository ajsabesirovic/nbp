using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class PlanExerciseDocument
{
    [BsonElement("exerciseId"), BsonRepresentation(BsonType.ObjectId)] public string ExerciseId { get; set; } = default!;
    [BsonElement("nameSnapshot")] public string? NameSnapshot { get; set; }
    [BsonElement("sets")] public int Sets { get; set; } = 3;
    [BsonElement("reps")] public string? Reps { get; set; }
    [BsonElement("restSeconds")] public int RestSeconds { get; set; } = 90;
    [BsonElement("notes")] public string? Notes { get; set; }
    [BsonElement("alternateExerciseIds"), BsonRepresentation(BsonType.ObjectId)]
    public List<string> AlternateExerciseIds { get; set; } = [];
}

[BsonIgnoreExtraElements]
public class PlanDayDocument
{
    [BsonElement("dayNumber")] public int DayNumber { get; set; }
    [BsonElement("name")] public string Name { get; set; } = default!;
    [BsonElement("exercises")] public List<PlanExerciseDocument> Exercises { get; set; } = [];
}

[BsonIgnoreExtraElements]
public class WorkoutPlanDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("authorId"), BsonRepresentation(BsonType.ObjectId)] public string AuthorId { get; set; } = default!;
    [BsonElement("authorName")] public string AuthorName { get; set; } = default!;
    [BsonElement("name")] public string Name { get; set; } = default!;
    [BsonElement("description")] public string? Description { get; set; }
    [BsonElement("durationWeeks")] public int DurationWeeks { get; set; }
    [BsonElement("level")] public string Level { get; set; } = default!;
    [BsonElement("goal")] public string Goal { get; set; } = default!;
    [BsonElement("daysPerWeek")] public int DaysPerWeek { get; set; }
    [BsonElement("visibility")] public string Visibility { get; set; } = "private";
    [BsonElement("status")] public string Status { get; set; } = "published";
    [BsonElement("days")] public List<PlanDayDocument> Days { get; set; } = [];
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; }
}
