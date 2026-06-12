using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace FitJourney.Infrastructure.MongoDB.Documents;

[BsonIgnoreExtraElements]
public class UserProfileDocument
{
    [BsonElement("gender")] public string? Gender { get; set; }
    [BsonElement("dateOfBirth")] public DateTime? DateOfBirth { get; set; }
    [BsonElement("heightCm")] public double? HeightCm { get; set; }
    [BsonElement("currentWeightKg")] public double? CurrentWeightKg { get; set; }
    [BsonElement("targetWeightKg")] public double? TargetWeightKg { get; set; }
    [BsonElement("experience")] public string? Experience { get; set; }
    [BsonElement("goal")] public string? Goal { get; set; }
}

[BsonIgnoreExtraElements]
public class UserDocument
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("name")] public string Name { get; set; } = default!;
    [BsonElement("email")] public string Email { get; set; } = default!;
    [BsonElement("passwordHash")] public string PasswordHash { get; set; } = default!;
    [BsonElement("role")] public string Role { get; set; } = "user";
    [BsonElement("avatarUrl")] public string? AvatarUrl { get; set; }
    [BsonElement("profile")] public UserProfileDocument? Profile { get; set; }
    [BsonElement("activePlanId"), BsonRepresentation(BsonType.ObjectId), BsonIgnoreIfNull]
    public string? ActivePlanId { get; set; }
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; }
}
