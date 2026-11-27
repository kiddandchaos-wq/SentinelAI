using Microsoft.AspNetCore.Mvc;
using SentinelAPI.Data;
using SentinelAPI.Models;
using UglyToad.PdfPig;

namespace SentinelAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KnowledgeController : ControllerBase
{
    private readonly SentinelDbContext _context;

    public KnowledgeController(SentinelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Knowledge API is working.");
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddDocument(
        [FromBody] KnowledgeDocument document)
    {
        _context.KnowledgeDocuments.Add(document);

        await _context.SaveChangesAsync();

        return Ok(document);
    }

    [HttpGet("documents")]
    public IActionResult GetDocuments()
    {
        return Ok(_context.KnowledgeDocuments.ToList());
    }

    [HttpGet("seed")]
    public async Task<IActionResult> Seed()
    {
        var document = new KnowledgeDocument
        {
            Title = "Employee Handbook",
            FileName = "handbook.pdf",
            Department = "HR",
            Owner = "Human Resources"
        };

        _context.KnowledgeDocuments.Add(document);

        await _context.SaveChangesAsync();

        return Ok(document);
    }
    [HttpGet("upload-test")]
    public IActionResult UploadTest()
    {
        return Ok("Upload endpoint loaded.");
    }
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads");

        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(
            uploadsFolder,
            file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var document = new KnowledgeDocument
        {
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            FileName = file.FileName,
            FilePath = filePath,
            Department = "Unknown",
            Owner = "Unknown"
        };

        _context.KnowledgeDocuments.Add(document);

        await _context.SaveChangesAsync();

        if (Path.GetExtension(file.FileName).ToLower() == ".pdf")
        {
            using var pdf = PdfDocument.Open(filePath);

            foreach (var page in pdf.GetPages())
            {
                if (!string.IsNullOrWhiteSpace(page.Text))
                {
                    _context.KnowledgeChunks.Add(new KnowledgeChunk
                    {
                        KnowledgeDocumentId = document.Id,
                        Content = page.Text
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        return Ok(document);
    }
    [HttpGet("upload-page")]
    public IActionResult UploadPage()
    {
        return Content(@"
    <html>
    <body>
        <h2>Upload Document</h2>
        <form action='/api/knowledge/upload' method='post' enctype='multipart/form-data'>
            <input type='file' name='file' />
            <button type='submit'>Upload</button>
        </form>
    </body>
    </html>", "text/html");
    }
    [HttpGet("chunks")]
    public IActionResult GetChunks()
    {
        return Ok(_context.KnowledgeChunks.ToList());
    }
    [HttpGet("search")]
    public IActionResult Search(string q)
    {
        var results = _context.KnowledgeChunks
            .Where(c => c.Content.Contains(q))
            .Take(10)
            .ToList();

        return Ok(results);
    }
    [HttpGet("ask")]
    public IActionResult Ask(string q)
    {
        var matches = _context.KnowledgeChunks
            .Where(c => c.Content.Contains(q))
            .Take(5)
            .Select(c => c.Content)
            .ToList();

        return Ok(new
        {
            Question = q,
            Matches = matches
        });
    }
}