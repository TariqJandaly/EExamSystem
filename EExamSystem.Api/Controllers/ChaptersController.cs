using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs.Chapters;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the creation, retrieval, updating, and deletion of testbank chapters.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1")]
public class ChaptersController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all chapters belonging to a specific testbank.
    /// </summary>
    /// <param name="testbankId">The ID of the parent testbank.</param>
    /// <returns>A list of ChapterDto objects.</returns>
    /// <response code="200">Returns the list of chapters successfully.</response>
    [HttpGet("testbanks/{testbankId}/chapters")]
    public async Task<IActionResult> GetChaptersForTestbank(int testbankId)
    {
        var chapters = await context.TestbankChapters
            .Where(c => c.TestbankId == testbankId)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

        return Ok(chapters);
    }

    /// <summary>
    /// Creates a new chapter inside a specific testbank.
    /// </summary>
    /// <param name="testbankId">The ID of the testbank.</param>
    /// <param name="newChapter">The chapter payload.</param>
    /// <response code="201">Returns the newly created chapter.</response>
    /// <response code="404">If the specified testbank does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("testbanks/{testbankId}/chapters")]
    public async Task<IActionResult> CreateChapter(int testbankId, [FromBody] ChapterCreateDto newChapter)
    {
        var testbankExists = await context.Testbanks.AnyAsync(t => t.Id == testbankId);
        if (!testbankExists) return NotFound($"Testbank with ID {testbankId} was not found.");

        var chapter = new TestbankChapter
        {
            TestbankId = testbankId,
            Name = newChapter.Name
        };

        context.TestbankChapters.Add(chapter);
        await context.SaveChangesAsync();

        var createdDto = new ChapterDto
        {
            Id = chapter.Id,
            Name = chapter.Name
        };

        return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, createdDto);
    }

    /// <summary>
    /// Retrieves a specific chapter by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the chapter.</param>
    /// <response code="200">Returns the requested chapter.</response>
    /// <response code="404">If the chapter does not exist.</response>
    [HttpGet("chapters/{id}")]
    public async Task<IActionResult> GetChapter(int id)
    {
        var chapter = await context.TestbankChapters
            .Where(c => c.Id == id)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .FirstOrDefaultAsync();

        if (chapter == null) return NotFound($"Chapter with ID {id} was not found.");
        return Ok(chapter);
    }

    /// <summary>
    /// Updates the name of an existing chapter.
    /// </summary>
    /// <param name="id">The ID of the chapter to update.</param>
    /// <param name="updatedChapter">The updated chapter payload.</param>
    /// <response code="204">Successfully updated the chapter.</response>
    /// <response code="404">If the chapter does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("chapters/{id}")]
    public async Task<IActionResult> UpdateChapter(int id, [FromBody] ChapterCreateDto updatedChapter)
    {
        var chapter = await context.TestbankChapters.FindAsync(id);
        if (chapter == null) return NotFound($"Chapter with ID {id} was not found.");

        chapter.Name = updatedChapter.Name;
        
        await context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Deletes a specific chapter. WARNING: This will cascade and delete all Questions inside it!
    /// </summary>
    /// <param name="id">The ID of the chapter to delete.</param>
    /// <response code="204">Successfully deleted the chapter.</response>
    /// <response code="404">If the chapter does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("chapters/{id}")]
    public async Task<IActionResult> DeleteChapter(int id)
    {
        var chapter = await context.TestbankChapters.FindAsync(id);
        if (chapter == null) return NotFound($"Chapter with ID {id} was not found.");

        context.TestbankChapters.Remove(chapter);
        await context.SaveChangesAsync();

        return NoContent();
    }
}