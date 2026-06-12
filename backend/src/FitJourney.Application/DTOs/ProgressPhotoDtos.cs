namespace FitJourney.Application.DTOs;

public record ProgressPhotoDto(string Id, string UserId, string Url, DateTime TakenAt, string? Note, DateTime CreatedAt);
