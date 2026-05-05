using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs.Exams;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the scheduling and configuration of exams for specific courses.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1")]
public class ExamsController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all scheduled exams for a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the parent course.</param>
    /// <returns>A list of ExamDto objects.</returns>
    /// <response code="200">Returns the list of exams successfully.</response>
    /// <response code="404">If the specified course does not exist.</response>
    [HttpGet("courses/{courseId}/exams")]
    public async Task<IActionResult> GetExamsForCourse(int courseId)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) return NotFound($"Course with ID {courseId} was not found.");

        var exams = await context.Exams
            .Where(e => e.CourseId == courseId)
            .Select(e => new ExamDto
            {
                Id = e.Id,
                Title = e.Title,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                DurationMinutes = e.DurationMinutes,
                MaxScore = e.MaxScore,
                PassingScore = e.PassingScore,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return Ok(exams);
    }

    /// <summary>
    /// Creates a new exam configuration within a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="newExam">The exam configuration details.</param>
    /// <returns>The newly created ExamDto.</returns>
    /// <response code="201">Returns the newly scheduled exam.</response>
    /// <response code="400">If the end time is before the start time, or passing score exceeds max score.</response>
    /// <response code="404">If the specified course does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("courses/{courseId}/exams")]
    public async Task<IActionResult> CreateExam(int courseId, [FromBody] ExamCreateDto newExam)
    {
        if (newExam.EndTime <= newExam.StartTime)
            return BadRequest("The exam's end time must be after its start time.");
            
        if (newExam.PassingScore > newExam.MaxScore)
            return BadRequest("The passing score cannot be higher than the maximum score.");

        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) return NotFound($"Course with ID {courseId} was not found.");

        var exam = new Exam
        {
            CourseId = courseId,
            Title = newExam.Title,
            StartTime = newExam.StartTime,
            EndTime = newExam.EndTime,
            DurationMinutes = newExam.DurationMinutes,
            MaxScore = newExam.MaxScore,
            PassingScore = newExam.PassingScore
        };

        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        var createdDto = new ExamDto
        {
            Id = exam.Id,
            Title = exam.Title,
            StartTime = exam.StartTime,
            EndTime = exam.EndTime,
            DurationMinutes = exam.DurationMinutes,
            MaxScore = exam.MaxScore,
            PassingScore = exam.PassingScore,
            CreatedAt = exam.CreatedAt
        };

        return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, createdDto);
    }

    /// <summary>
    /// Retrieves a specific exam by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the exam.</param>
    /// <returns>A single ExamDto object.</returns>
    /// <response code="200">Returns the requested exam successfully.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [HttpGet("exams/{id}")]
    public async Task<IActionResult> GetExam(int id)
    {
        var exam = await context.Exams
            .Where(e => e.Id == id)
            .Select(e => new ExamDto
            {
                Id = e.Id,
                Title = e.Title,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                DurationMinutes = e.DurationMinutes,
                MaxScore = e.MaxScore,
                PassingScore = e.PassingScore,
                CreatedAt = e.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (exam == null) return NotFound($"Exam with ID {id} was not found.");
        return Ok(exam);
    }

    /// <summary>
    /// Updates an exam's schedule or configuration.
    /// </summary>
    /// <param name="id">The ID of the exam to update.</param>
    /// <param name="updatedExam">The updated exam payload.</param>
    /// <response code="204">Successfully updated the exam.</response>
    /// <response code="400">If the end time is before the start time, or passing score exceeds max score.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("exams/{id}")]
    public async Task<IActionResult> UpdateExam(int id, [FromBody] ExamCreateDto updatedExam)
    {
        if (updatedExam.EndTime <= updatedExam.StartTime)
            return BadRequest("The exam's end time must be after its start time.");
            
        if (updatedExam.PassingScore > updatedExam.MaxScore)
            return BadRequest("The passing score cannot be higher than the maximum score.");

        var exam = await context.Exams.FindAsync(id);
        if (exam == null) return NotFound($"Exam with ID {id} was not found.");

        exam.Title = updatedExam.Title;
        exam.StartTime = updatedExam.StartTime;
        exam.EndTime = updatedExam.EndTime;
        exam.DurationMinutes = updatedExam.DurationMinutes;
        exam.MaxScore = updatedExam.MaxScore;
        exam.PassingScore = updatedExam.PassingScore;

        await context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Deletes an exam.
    /// </summary>
    /// <param name="id">The ID of the exam to delete.</param>
    /// <response code="204">Successfully deleted the exam.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{id}")]
    public async Task<IActionResult> DeleteExam(int id)
    {
        var exam = await context.Exams.FindAsync(id);
        if (exam == null) return NotFound($"Exam with ID {id} was not found.");

        context.Exams.Remove(exam);
        await context.SaveChangesAsync();

        return NoContent();
    }
}