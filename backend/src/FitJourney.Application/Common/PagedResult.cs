namespace FitJourney.Application.Common;

public record PagedResult<T>(List<T> Items, long Total, int Page, int Limit);
