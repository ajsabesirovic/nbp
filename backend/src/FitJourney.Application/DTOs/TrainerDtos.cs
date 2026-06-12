namespace FitJourney.Application.DTOs;

public record TrainerProfileDto(
    string Id,
    string UserId,
    List<string> Certifications,
    string? Specialization,
    double? PricePerPlan,
    string? Bio,
    List<string> ClientIds,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record UpdateTrainerProfileRequest(
    List<string>? Certifications,
    string? Specialization,
    double? PricePerPlan,
    string? Bio);

public record ClientSummaryDto(string Id, string Name, string Email);

public record PlanAssignedClientDto(string Id, string Name, string Email, DateTime AssignedAt);

public record PlanCompletionDto(
    string Id,
    string Name,
    int ExpectedSessions,
    long LoggedSessions,
    double CompletionRate);

public record ClientDetailDto(
    ClientSummaryDto Client,
    UserProfileDto? Profile,
    List<PlanCompletionDto> Completion,
    List<SessionDto> RecentSessions,
    List<ProgressPhotoDto> Photos);

public record AssignPlanRequest(string ClientId);
public record ManageClientRequest(string ClientId);
