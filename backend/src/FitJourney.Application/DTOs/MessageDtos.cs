namespace FitJourney.Application.DTOs;

public record MessageDto(
    string Id,
    string FromUserId,
    string ToUserId,
    string FromName,
    string ToName,
    string Body,
    DateTime CreatedAt,
    DateTime? ReadAt);

public record SendMessageRequest(string ToUserId, string Body);
public record ThreadSummaryDto(string OtherUserId, string OtherUserName, string LastBody, DateTime LastAt, int UnreadCount);
