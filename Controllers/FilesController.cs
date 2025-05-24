using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // <-- Important!
using SecureDocumentStorageSystem.Data;
using SecureDocumentStorageSystem.Models;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file selected");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var data = new byte[file.Length];
        await file.OpenReadStream().ReadAsync(data);

        int version = await _context.Documents
            .Where(d => d.UserId == userId && d.Name == file.FileName)
            .MaxAsync(d => (int?)d.Version) ?? 0;

        var document = new Document
        {
            UserId = userId,
            Name = file.FileName,
            Version = version + 1,
            Data = data,
            UploadDate = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return Ok(new { message = "File uploaded successfully" });
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var files = await _context.Documents
            .Where(d => d.UserId == userId)
            .GroupBy(d => d.Name)
            .Select(g => g.OrderByDescending(d => d.Version).First())
            .Select(d => new { d.Name, d.Version, d.UploadDate })
            .ToListAsync();

        return Ok(files);
    }

    [HttpGet("{filename}")]
    public async Task<IActionResult> Download(string filename)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var document = await _context.Documents
            .Where(d => d.UserId == userId && d.Name == filename)
            .OrderByDescending(d => d.Version)
            .FirstOrDefaultAsync();

        if (document == null)
            return NotFound("File not found");

        return File(document.Data, "application/octet-stream", document.Name);
    }
}
