using System.Security.Claims;
using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using FitJourney.Application.DTOs;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/photos")]
[Authorize]
public class ProgressPhotosController(IProgressPhotoRepository repo, IMapper mapper, IWebHostEnvironment env) : ControllerBase
{
    private string Uid() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int limit = 100)
    {
        var items = await repo.GetByUserAsync(Uid(), limit);
        return Ok(new { items = mapper.Map<List<ProgressPhotoDto>>(items) });
    }

    [HttpPost]
    [RequestSizeLimit(10_000_000)]
    [ProducesResponseType(typeof(ProgressPhotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] UploadPhotoForm form)
    {
        var (file, takenAt, note) = (form.File, form.TakenAt, form.Note);
        if (file == null || file.Length == 0) return BadRequest(new { error = "No file" });
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            return BadRequest(new { error = "Unsupported file type" });

        if (takenAt.HasValue && takenAt.Value.ToUniversalTime() > DateTime.UtcNow.AddMinutes(5))
            return BadRequest(new { error = "Date taken cannot be in the future" });

        var uid = Uid();
        string url;
        var storage = HttpContext.RequestServices.GetService<IPhotoStorage>();
        if (storage != null)
        {
            await using var stream = file.OpenReadStream();
            url = await storage.SaveAsync(stream, uid, file.FileName);
        }
        else
        {
            var folder = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", uid);
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(folder, fileName);
            using (var stream = System.IO.File.Create(path))
                await file.CopyToAsync(stream);
            url = $"/uploads/{uid}/{fileName}";
        }
        var now = DateTime.UtcNow;
        var photo = new ProgressPhoto
        {
            UserId = uid,
            Url = url,
            TakenAt = takenAt ?? now,
            Note = note,
            CreatedAt = now,
        };
        var saved = await repo.CreateAsync(photo);
        return Ok(mapper.Map<ProgressPhotoDto>(saved));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await repo.DeleteAsync(id, Uid());
        return NoContent();
    }
}

public class UploadPhotoForm
{

    public IFormFile File { get; set; } = default!;

    public DateTime? TakenAt { get; set; }

    public string? Note { get; set; }
}
