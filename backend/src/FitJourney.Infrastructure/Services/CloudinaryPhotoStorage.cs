using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FitJourney.Domain.Interfaces;

namespace FitJourney.Infrastructure.Services;

public class CloudinaryPhotoStorage : IPhotoStorage
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryPhotoStorage(string cloudinaryUrl)
    {
        _cloudinary = new Cloudinary(cloudinaryUrl) { Api = { Secure = true } };
    }

    public async Task<string> SaveAsync(Stream content, string userId, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = $"fitjourney/{userId}",
            PublicId = Guid.NewGuid().ToString("N"),
        };
        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
        return result.SecureUrl.ToString();
    }
}
