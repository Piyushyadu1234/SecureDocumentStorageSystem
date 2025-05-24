using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SecureDocumentStorageSystem.Data;
using SecureDocumentStorageSystem.Models;
using Microsoft.EntityFrameworkCore;


[Authorize]
[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var userDocs = _context.Documents.Where(d => d.UserId == userId && d.Name == file.FileName);
        var version = await userDocs.CountAsync();

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var document = new Document
        {
            UserId = userId,
            Name = file.FileName,
            Version = version,
            Data = ms.ToArray(),
            UploadDate = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return Ok(new { message = "File uploaded" });
    }

    [HttpGet("{filename}")]
    public async Task<IActionResult> Download(string filename, [FromQuery] int? revision)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var files = _context.Documents.Where(d => d.UserId == userId && d.Name == filename);

        var file = revision.HasValue
            ? await files.OrderBy(d => d.Version).Skip(revision.Value).FirstOrDefaultAsync()
            : await files.OrderByDescending(d => d.Version).FirstOrDefaultAsync();

        if (file == null) return NotFound();
        return File(file.Data, "application/octet-stream", file.Name);
    }
}
