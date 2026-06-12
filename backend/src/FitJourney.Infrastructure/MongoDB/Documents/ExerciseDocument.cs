using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class ExerciseDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("name")] public string Name { get; set; } = default!;
    [BsonElement("type")] public string Type { get; set; } = default!;
    [BsonElement("primaryMuscles")] public List<string> PrimaryMuscles { get; set; } = [];
    [BsonElement("secondaryMuscles")] public List<string> SecondaryMuscles { get; set; } = [];
    [BsonElement("category")] public string? Category { get; set; }
    [BsonElement("equipment")] public string? Equipment { get; set; }
    [BsonElement("difficulty")] public int? Difficulty { get; set; }
    [BsonElement("description")] public string? Description { get; set; }
    [BsonElement("instructions")] public string? Instructions { get; set; }
    [BsonElement("imageUrl")] public string? ImageUrl { get; set; }
    [BsonElement("videoUrl")] public string? VideoUrl { get; set; }
    [BsonElement("isCustom")] public bool IsCustom { get; set; }
    [BsonElement("createdBy")] public string? CreatedBy { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
}
