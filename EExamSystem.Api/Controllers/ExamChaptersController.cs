using EExamSystem.Api.Data;
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
    public async Task<IActionResult> GetChaptersForExam(int examId)
    {
        var exam = await context.Exams
            .Include(e => e.CoveredChapters)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return NotFound($"Exam with ID {examId} was not found.");

        var chapters = exam.CoveredChapters.Select(c => new ChapterDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return Ok(chapters);
    }

    /// <summary>
    /// Attaches a specific testbank chapter to an exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="chapterId">The ID of the chapter to attach.</param>
    /// <response code="204">Successfully attached the chapter to the exam.</response>
    /// <response code="400">If the chapter is already attached to this exam.</response>
    /// <response code="404">If the exam or the chapter does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("exams/{examId}/chapters/{chapterId}")]
    public async Task<IActionResult> AttachChapterToExam(int examId, int chapterId)
    {
        var exam = await context.Exams
            .Include(e => e.CoveredChapters)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return NotFound($"Exam with ID {examId} was not found.");

        var chapter = await context.TestbankChapters.FindAsync(chapterId);
        if (chapter == null) return NotFound($"Chapter with ID {chapterId} was not found.");

        if (exam.CoveredChapters.Any(c => c.Id == chapterId))
        {
            return BadRequest($"Chapter {chapterId} is already attached to Exam {examId}.");
        }

        exam.CoveredChapters.Add(chapter);
        await context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Removes a testbank chapter from an exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="chapterId">The ID of the chapter to remove.</param>
    /// <response code="204">Successfully removed the chapter from the exam.</response>
    /// <response code="404">If the exam or chapter does not exist, or if the chapter wasn't attached.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{examId}/chapters/{chapterId}")]
    public async Task<IActionResult> RemoveChapterFromExam(int examId, int chapterId)
    {
        var exam = await context.Exams
            .Include(e => e.CoveredChapters)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return NotFound($"Exam with ID {examId} was not found.");

        var chapterToRemove = exam.CoveredChapters.FirstOrDefault(c => c.Id == chapterId);
        
        if (chapterToRemove == null) 
            return NotFound($"Chapter {chapterId} is not currently attached to Exam {examId}.");

        exam.CoveredChapters.Remove(chapterToRemove);
        await context.SaveChangesAsync();

        return NoContent();
    }
}