using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;
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
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="ExamDto"/>.</returns>
    /// <response code="200">Successfully retrieved the exam list.</response>
    /// <response code="404">
    /// The course was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseNotFound</c> – No course exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("courses/{courseId}/exams")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ExamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ExamDto>>>> GetExamsForCourse(int courseId)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return NotFound(new ErrorServiceResponse { Message = "CourseNotFound", StatusCode = 404 });

        var exams = await context.Exams
            .Where(e => e.CourseId == courseId)
            .Select(e => new ExamDto
            {
                Id = e.Id, Title = e.Title, StartTime = e.StartTime, EndTime = e.EndTime,
                DurationMinutes = e.DurationMinutes, MaxScore = e.MaxScore,
                PassingScore = e.PassingScore, CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return Ok(new ServiceResponse<IEnumerable<ExamDto>> { Data = exams });
    }

    /// <summary>
    /// Creates a new exam configuration within a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the parent course.</param>
    /// <param name="newExam">The exam configuration payload. See <see cref="ExamCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the newly created <see cref="ExamDto"/>.</returns>
    /// <response code="201">
    /// Exam created successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamCreatedSuccess</c> – The exam was persisted and is ready for scheduling.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// Validation failed. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InvalidExamTimeRange</c> – <c>EndTime</c> is not after <c>StartTime</c>.</item>
    ///   <item><c>InvalidPassingScore</c> – <c>PassingScore</c> exceeds <c>MaxScore</c>.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The parent course was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseNotFound</c> – No course exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("courses/{courseId}/exams")]
    [ProducesResponseType(typeof(ServiceResponse<ExamDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ExamDto>>> CreateExam(int courseId, [FromBody] ExamCreateDto newExam)
    {
        if (newExam.EndTime <= newExam.StartTime)
            return BadRequest(new ErrorServiceResponse { Message = "InvalidExamTimeRange", StatusCode = 400 });

        if (newExam.PassingScore > newExam.MaxScore)
            return BadRequest(new ErrorServiceResponse { Message = "InvalidPassingScore", StatusCode = 400 });

        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return NotFound(new ErrorServiceResponse { Message = "CourseNotFound", StatusCode = 404 });

        var exam = new Exam
        {
            CourseId = courseId, Title = newExam.Title, StartTime = newExam.StartTime,
            EndTime = newExam.EndTime, DurationMinutes = newExam.DurationMinutes,
            MaxScore = newExam.MaxScore, PassingScore = newExam.PassingScore
        };

        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        var createdDto = new ExamDto
        {
            Id = exam.Id, Title = exam.Title, StartTime = exam.StartTime, EndTime = exam.EndTime,
            DurationMinutes = exam.DurationMinutes, MaxScore = exam.MaxScore,
            PassingScore = exam.PassingScore, CreatedAt = exam.CreatedAt
        };

        return CreatedAtAction(nameof(GetExam), new { id = exam.Id },
            new ServiceResponse<ExamDto> { Data = createdDto, Message = "ExamCreatedSuccess", StatusCode = 201 });
    }

    /// <summary>
    /// Retrieves a specific exam by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the exam.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the matching <see cref="ExamDto"/>.</returns>
    /// <response code="200">Successfully retrieved the exam.</response>
    /// <response code="404">
    /// The exam was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("exams/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<ExamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ExamDto>>> GetExam(int id)
    {
        var exam = await context.Exams
            .Where(e => e.Id == id)
            .Select(e => new ExamDto
            {
                Id = e.Id, Title = e.Title, StartTime = e.StartTime, EndTime = e.EndTime,
                DurationMinutes = e.DurationMinutes, MaxScore = e.MaxScore,
                PassingScore = e.PassingScore, CreatedAt = e.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (exam == null)
            return NotFound(new ErrorServiceResponse { Message = "ExamNotFound", StatusCode = 404 });

        return Ok(new ServiceResponse<ExamDto> { Data = exam });
    }

    /// <summary>
    /// Updates an exam's schedule or configuration.
    /// </summary>
    /// <param name="id">The ID of the exam to update.</param>
    /// <param name="updatedExam">The updated exam payload. See <see cref="ExamCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Exam updated successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamUpdatedSuccess</c> – Changes were persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// Validation failed. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InvalidExamTimeRange</c> – <c>EndTime</c> is not after <c>StartTime</c>.</item>
    ///   <item><c>InvalidPassingScore</c> – <c>PassingScore</c> exceeds <c>MaxScore</c>.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The exam was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("exams/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateExam(int id, [FromBody] ExamCreateDto updatedExam)
    {
        if (updatedExam.EndTime <= updatedExam.StartTime)
            return BadRequest(new ErrorServiceResponse { Message = "InvalidExamTimeRange", StatusCode = 400 });

        if (updatedExam.PassingScore > updatedExam.MaxScore)
            return BadRequest(new ErrorServiceResponse { Message = "InvalidPassingScore", StatusCode = 400 });

        var exam = await context.Exams.FindAsync(id);
        if (exam == null)
            return NotFound(new ErrorServiceResponse { Message = "ExamNotFound", StatusCode = 404 });

        exam.Title = updatedExam.Title;
        exam.StartTime = updatedExam.StartTime;
        exam.EndTime = updatedExam.EndTime;
        exam.DurationMinutes = updatedExam.DurationMinutes;
        exam.MaxScore = updatedExam.MaxScore;
        exam.PassingScore = updatedExam.PassingScore;

        await context.SaveChangesAsync();
        return Ok(new ServiceResponse<bool> { Data = true, Message = "ExamUpdatedSuccess" });
    }

    /// <summary>
    /// Permanently deletes an exam and all associated session data.
    /// </summary>
    /// <param name="id">The ID of the exam to delete.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Exam deleted successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamDeletedSuccess</c> – The exam was removed.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The exam was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteExam(int id)
    {
        var exam = await context.Exams.FindAsync(id);
        if (exam == null)
            return NotFound(new ErrorServiceResponse { Message = "ExamNotFound", StatusCode = 404 });

        context.Exams.Remove(exam);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "ExamDeletedSuccess" });
    }
}