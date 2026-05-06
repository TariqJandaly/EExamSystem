using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;
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
    /// <response code="404">If the specified testbank does not exist.</response>
    [HttpGet("testbanks/{testbankId}/chapters")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ChapterDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ChapterDto>>>> GetChaptersForTestbank(int testbankId)
    {
        var testbankExists = await context.Testbanks.AnyAsync(t => t.Id == testbankId);
        if (!testbankExists) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "TestbankNotFound", StatusCode = 404 });
        
        var chapters = await context.TestbankChapters
            .Where(c => c.TestbankId == testbankId)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
            
        return Ok(new ServiceResponse<IEnumerable<ChapterDto>> { Data = chapters });
    }

    /// <summary>
    /// Creates a new chapter inside a specific testbank.
    /// </summary>
    /// <param name="testbankId">The ID of the testbank.</param>
    /// <param name="newChapter">The chapter payload.</param>
    /// <returns>The newly created ChapterDto.</returns>
    /// <response code="201">Returns the newly created chapter.</response>
    /// <response code="404">If the specified testbank does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("testbanks/{testbankId}/chapters")]
    [ProducesResponseType(typeof(ServiceResponse<ChapterDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ChapterDto>>> CreateChapter(int testbankId, [FromBody] ChapterCreateDto newChapter)
    {
        var testbankExists = await context.Testbanks.AnyAsync(t => t.Id == testbankId);
        if (!testbankExists) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "TestbankNotFound", StatusCode = 404 });

        var chapter = new TestbankChapter
        {
            TestbankId = testbankId,
            Name = newChapter.Name,
            Description = newChapter.Description
        };

        context.TestbankChapters.Add(chapter);
        await context.SaveChangesAsync();

        var createdDto = new ChapterDto
        {
            Id = chapter.Id,
            Name = chapter.Name,
            Description = chapter.Description,
            CreatedAt = chapter.CreatedAt
        };

        return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, new ServiceResponse<ChapterDto> { Data = createdDto, Message = "ChapterCreatedSuccess", StatusCode = 201 });
    }

    /// <summary>
    /// Retrieves a specific chapter by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the chapter.</param>
    /// <returns>A single ChapterDto object.</returns>
    /// <response code="200">Returns the requested chapter.</response>
    /// <response code="404">If the chapter does not exist.</response>
    [HttpGet("chapters/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<ChapterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ChapterDto>>> GetChapter(int id)
    {
        var chapter = await context.TestbankChapters
            .Where(c => c.Id == id)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (chapter == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ChapterNotFound", StatusCode = 404 });
            
        return Ok(new ServiceResponse<ChapterDto> { Data = chapter });
    }

    /// <summary>
    /// Updates the name and description of an existing chapter.
    /// </summary>
    /// <param name="id">The ID of the chapter to update.</param>
    /// <param name="updatedChapter">The updated chapter payload.</param>
    /// <response code="200">Successfully updated the chapter.</response>
    /// <response code="404">If the chapter does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("chapters/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateChapter(int id, [FromBody] ChapterCreateDto updatedChapter)
    {
        var chapter = await context.TestbankChapters.FindAsync(id);
        if (chapter == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ChapterNotFound", StatusCode = 404 });

        chapter.Name = updatedChapter.Name;
        chapter.Description = updatedChapter.Description;
        
        await context.SaveChangesAsync();
        return Ok(new ServiceResponse<bool> { Data = true, Message = "ChapterUpdatedSuccess" });
    }

    /// <summary>
    /// Deletes a specific chapter. WARNING: This will cascade and delete all Questions inside it!
    /// </summary>
    /// <param name="id">The ID of the chapter to delete.</param>
    /// <response code="200">Successfully deleted the chapter.</response>
    /// <response code="404">If the chapter does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("chapters/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteChapter(int id)
    {
        var chapter = await context.TestbankChapters.FindAsync(id);
        if (chapter == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ChapterNotFound", StatusCode = 404 });

        context.TestbankChapters.Remove(chapter);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "ChapterDeletedSuccess" });
    }
}