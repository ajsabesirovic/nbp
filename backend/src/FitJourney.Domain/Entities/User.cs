using FitJourney.Domain.Enums;
namespace FitJourney.Domain.Entities;

public class UserProfile
{
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public double? HeightCm { get; set; }
    public double? CurrentWeightKg { get; set; }
    public double? TargetWeightKg { get; set; }
    public string? Experience { get; set; }
    public string? Goal { get; set; }
}

public class User
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.user;
    public string? AvatarUrl { get; set; }
    public UserProfile? Profile { get; set; }
    public string? ActivePlanId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
