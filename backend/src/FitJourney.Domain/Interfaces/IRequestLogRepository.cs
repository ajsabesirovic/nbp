using FitJourney.Domain.Entities;
namespace FitJourney.Domain.Interfaces;

public interface IRequestLogRepository
{
    Task InsertAsync(RequestLog log);
    Task<(List<RequestLog> Items, long Total)> RecentAsync(int page, int limit);
    Task<(List<RequestLog> Items, long Total)> SlowAsync(long thresholdMs, int page, int limit);
}
