using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Chapters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the linkage between Exams and Testbank Chapters.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1")]
public class ExamChaptersController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all chapters currently attached to a specific exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <returns>A list of ChapterDto objects.</returns>
    /// <response code="200">Returns the list of attached chapters successfully.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [HttpGet("exams/{examId}/chapters")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ChapterDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ChapterDto>>>> GetChaptersForExam(int examId)
    {
        var exam = await context.Exams
            .Include(e => e.CoveredChapters)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ExamNotFound", StatusCode = 404 });

        var chapters = exam.CoveredChapters.Select(c => new ChapterDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return Ok(new ServiceResponse<IEnumerable<ChapterDto>> { Data = chapters });
    }

    /// <summary>
    /// Attaches a specific testbank chapter to an exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="chapterId">The ID of the chapter to attach.</param>
    /// <response code="200">Successfully attached the chapter to the exam.</response>
    /// <response code="400">If the chapter is already attached to this exam.</response>
    /// <response code="404">If the exam or the chapter does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("exams/{examId}/chapters/{chapterId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> AttachChapterToExam(int examId, int chapterId)
    {
        var exam = await context.Exams
            .Include(e => e.CoveredChapters)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ExamNotFound", StatusCode = 404 });

        var chapter = await context.TestbankChapters.FindAsync(chapterId);
        if (chapter == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ChapterNotFound", StatusCode = 404 });

        if (exam.CoveredChapters.Any(c => c.Id == chapterId))
        {
            return BadRequest(new ErrorServiceResponse { Success = false, Message = "ChapterAlreadyAttached", StatusCode = 400 });
        }

        exam.CoveredChapters.Add(chapter);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "ChapterAttachedSuccess" });
    }

    /// <summary>
    /// Removes a testbank chapter from an exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="chapterId">The ID of the chapter to remove.</param>
    /// <response code="200">Successfully removed the chapter from the exam.</response>
    /// <response code="404">If the exam or chapter does not exist, or if the chapter wasn't attached.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{examId}/chapters/{chapterId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> RemoveChapterFromExam(int examId, int chapterId)
    {
        var exam = await context.Exams
            .Include(e => e.CoveredChapters)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ExamNotFound", StatusCode = 404 });

        var chapterToRemove = exam.CoveredChapters.FirstOrDefault(c => c.Id == chapterId);
        
        if (chapterToRemove == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = "ChapterNotAttached", StatusCode = 404 });

        exam.CoveredChapters.Remove(chapterToRemove);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "ChapterRemovedSuccess" });
    }
}