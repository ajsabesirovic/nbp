using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class PlanAssignmentDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("planId"), BsonRepresentation(BsonType.ObjectId)] public string PlanId { get; set; } = default!;
    [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)] public string UserId { get; set; } = default!;
    [BsonElement("assignedBy"), BsonRepresentation(BsonType.ObjectId)] public string AssignedBy { get; set; } = default!;
    [BsonElement("status")] public string Status { get; set; } = "active";
    [BsonElement("assignedAt")] public DateTime AssignedAt { get; set; }
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; }
}
