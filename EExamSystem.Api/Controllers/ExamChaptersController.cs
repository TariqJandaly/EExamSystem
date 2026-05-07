using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Chapters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the linkage between exams and testbank chapters, controlling which question pools are eligible for an exam.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1")]
public class ExamChaptersController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all testbank chapters currently attached to a specific exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="ChapterDto"/>.</returns>
    /// <response code="200">Successfully retrieved the list of attached chapters.</response>
    /// <response code="404">
    /// The exam was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("exams/{examId}/chapters")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ChapterDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ChapterDto>>>> GetChaptersForExam(int examId)
    {
        var exam = await context.Exams.Include(e => e.CoveredChapters).FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null)
            return NotFound(new ErrorServiceResponse { Message = "ExamNotFound", StatusCode = 404 });

        var chapters = exam.CoveredChapters.Select(c => new ChapterDto { Id = c.Id, Name = c.Name }).ToList();
        return Ok(new ServiceResponse<IEnumerable<ChapterDto>> { Data = chapters });
    }

    /// <summary>
    /// Attaches a testbank chapter to an exam, adding its questions to the eligible pool.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="chapterId">The ID of the chapter to attach.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Chapter attached successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ChapterAttachedSuccess</c> – The chapter is now linked to the exam.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// Attachment rejected. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ChapterAlreadyAttached</c> – This chapter is already linked to the exam.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    ///   <item><c>ChapterNotFound</c> – No chapter exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("exams/{examId}/chapters/{chapterId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> AttachChapterToExam(int examId, int chapterId)
    {
        var exam = await context.Exams.Include(e => e.CoveredChapters).FirstOrDefaultAsync(e => e.Id == examId);
        if (exam == null)
            return NotFound(new ErrorServiceResponse { Message = "ExamNotFound", StatusCode = 404 });

        var chapter = await context.TestbankChapters.FindAsync(chapterId);
        if (chapter == null)
            return NotFound(new ErrorServiceResponse { Message = "ChapterNotFound", StatusCode = 404 });

        if (exam.CoveredChapters.Any(c => c.Id == chapterId))
            return BadRequest(new ErrorServiceResponse { Message = "ChapterAlreadyAttached", StatusCode = 400 });

        exam.CoveredChapters.Add(chapter);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "ChapterAttachedSuccess" });
    }

    /// <summary>
    /// Removes a testbank chapter from an exam, excluding its questions from the eligible pool.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="chapterId">The ID of the chapter to remove.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Chapter removed successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ChapterRemovedSuccess</c> – The chapter was unlinked from the exam.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    ///   <item><c>ChapterNotAttached</c> – The chapter is not currently linked to this exam.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{examId}/chapters/{chapterId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> RemoveChapterFromExam(int examId, int chapterId)
    {
        var exam = await context.Exams.Include(e => e.CoveredChapters).FirstOrDefaultAsync(e => e.Id == examId);
        if (exam == null)
            return NotFound(new ErrorServiceResponse { Message = "ExamNotFound", StatusCode = 404 });

        var chapterToRemove = exam.CoveredChapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapterToRemove == null)
            return NotFound(new ErrorServiceResponse { Message = "ChapterNotAttached", StatusCode = 404 });

        exam.CoveredChapters.Remove(chapterToRemove);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "ChapterRemovedSuccess" });
    }
}