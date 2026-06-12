namespace FitJourney.Application.DTOs;

public record AdminUserDto(
    string Id,
    string Name,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record SetUserRoleRequest(string Role);

public record ModeratePlanRequest(string Status);

public record RequestLogDto(
    string Id,
    string Method,
    string Path,
    int StatusCode,
    long DurationMs,
    string? UserId,
    string RequestId,
    bool SlowRequest,
    DateTime Timestamp);

public record AdminStatsDto(
    long TotalUsers,
    long RegularUsers,
    long Trainers,
    long Admins,
    long NewUsersLast7Days,
    long TotalSessions,
    long SessionsLast7Days,
    long ActiveUsersLast7Days,
    long TotalPlans,
    long PublishedPublicPlans);

public record AdminTrainerClientDto(string Id, string Name, string Email, DateTime JoinedAt);

public record AdminTrainerDto(
    string UserId,
    string Name,
    string Email,
    string? Specialization,
    int ClientCount,
    List<AdminTrainerClientDto> Clients);
