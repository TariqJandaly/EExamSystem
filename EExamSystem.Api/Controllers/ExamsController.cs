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
    /// <returns>A list of ExamDto objects.</returns>
    /// <response code="200">Returns the list of exams successfully.</response>
    /// <response code="404">If the specified course does not exist.</response>
    [HttpGet("courses/{courseId}/exams")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ExamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ExamDto>>>> GetExamsForCourse(int courseId)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = $"Course with ID {courseId} was not found.", StatusCode = 404 });

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

        return Ok(new ServiceResponse<IEnumerable<ExamDto>> { Data = exams });
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
    [ProducesResponseType(typeof(ServiceResponse<ExamDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ExamDto>>> CreateExam(int courseId, [FromBody] ExamCreateDto newExam)
    {
        if (newExam.EndTime <= newExam.StartTime)
            return BadRequest(new ErrorServiceResponse { Success = false, Message = "The exam's end time must be after its start time.", StatusCode = 400 });
            
        if (newExam.PassingScore > newExam.MaxScore)
            return BadRequest(new ErrorServiceResponse { Success = false, Message = "The passing score cannot be higher than the maximum score.", StatusCode = 400 });

        var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = $"Course with ID {courseId} was not found.", StatusCode = 404 });

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

        return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, new ServiceResponse<ExamDto> { Data = createdDto, Message = "Exam created successfully.", StatusCode = 201 });
    }

    /// <summary>
    /// Retrieves a specific exam by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the exam.</param>
    /// <returns>A single ExamDto object.</returns>
    /// <response code="200">Returns the requested exam successfully.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [HttpGet("exams/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<ExamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ExamDto>>> GetExam(int id)
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

        if (exam == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = $"Exam with ID {id} was not found.", StatusCode = 404 });
            
        return Ok(new ServiceResponse<ExamDto> { Data = exam });
    }

    /// <summary>
    /// Updates an exam's schedule or configuration.
    /// </summary>
    /// <param name="id">The ID of the exam to update.</param>
    /// <param name="updatedExam">The updated exam payload.</param>
    /// <response code="200">Successfully updated the exam.</response>
    /// <response code="400">If the end time is before the start time, or passing score exceeds max score.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("exams/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateExam(int id, [FromBody] ExamCreateDto updatedExam)
    {
        if (updatedExam.EndTime <= updatedExam.StartTime)
            return BadRequest(new ErrorServiceResponse { Success = false, Message = "The exam's end time must be after its start time.", StatusCode = 400 });
            
        if (updatedExam.PassingScore > updatedExam.MaxScore)
            return BadRequest(new ErrorServiceResponse { Success = false, Message = "The passing score cannot be higher than the maximum score.", StatusCode = 400 });

        var exam = await context.Exams.FindAsync(id);
        if (exam == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = $"Exam with ID {id} was not found.", StatusCode = 404 });

        exam.Title = updatedExam.Title;
        exam.StartTime = updatedExam.StartTime;
        exam.EndTime = updatedExam.EndTime;
        exam.DurationMinutes = updatedExam.DurationMinutes;
        exam.MaxScore = updatedExam.MaxScore;
        exam.PassingScore = updatedExam.PassingScore;

        await context.SaveChangesAsync();
        return Ok(new ServiceResponse<bool> { Data = true, Message = "Exam updated successfully." });
    }

    /// <summary>
    /// Deletes an exam.
    /// </summary>
    /// <param name="id">The ID of the exam to delete.</param>
    /// <response code="200">Successfully deleted the exam.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteExam(int id)
    {
        var exam = await context.Exams.FindAsync(id);
        if (exam == null) 
            return NotFound(new ErrorServiceResponse { Success = false, Message = $"Exam with ID {id} was not found.", StatusCode = 404 });

        context.Exams.Remove(exam);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "Exam deleted successfully." });
    }
}