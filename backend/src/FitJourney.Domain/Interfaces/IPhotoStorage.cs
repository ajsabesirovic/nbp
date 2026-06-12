namespace FitJourney.Domain.Interfaces;

public interface IPhotoStorage
{
    Task<string> SaveAsync(Stream content, string userId, string fileName);
}
