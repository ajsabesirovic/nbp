namespace FitJourney.Application.DTOs;

public record NotificationDto(
    string Id,
    string UserId,
    string Type,
    string Title,
    string? Body,
    string? Link,
    DateTime CreatedAt,
    DateTime? ReadAt);
