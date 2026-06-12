namespace FitJourney.Application.DTOs;

public record RegisterRequest(string Name, string Email, string Password, string? Role = null);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);

public record UserProfileDto(
    string? Gender,
    DateTime? DateOfBirth,
    double? HeightCm,
    double? CurrentWeightKg,
    double? TargetWeightKg,
    string? Experience,
    string? Goal);

public record UserDto(
    string Id,
    string Name,
    string Email,
    string Role,
    string? AvatarUrl,
    UserProfileDto? Profile,
    string? ActivePlanId);

public record UpdateProfileRequest(string? Name, UserProfileDto? Profile);
public record SetActivePlanRequest(string? PlanId);
public record MeResponse(UserDto User);
