using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Courses;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the creation, retrieval, updating, and deletion of university courses.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CoursesController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all courses in the system.
    /// </summary>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="CourseDto"/>.</returns>
    /// <response code="200">Successfully retrieved the full course list.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<CourseDto>>>> GetCourses()
    {
        var courses = await context.Courses
            .Select(c => new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name })
            .ToListAsync();

        return Ok(new ServiceResponse<IEnumerable<CourseDto>> { Data = courses });
    }

    /// <summary>
    /// Retrieves the details of a specific course by its ID.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the matching <see cref="CourseDto"/>.</returns>
    /// <response code="200">Successfully retrieved the course.</response>
    /// <response code="404">
    /// The course was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseNotFound</c> – No course exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("{courseId}")]
    [ProducesResponseType(typeof(ServiceResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<CourseDto>>> GetCourse(int courseId)
    {
        var course = await context.Courses
            .Where(c => c.Id == courseId)
            .Select(c => new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name })
            .FirstOrDefaultAsync();

        if (course == null)
            return NotFound(new ErrorServiceResponse { Message = "CourseNotFound", StatusCode = 404 });

        return Ok(new ServiceResponse<CourseDto> { Data = course });
    }

    /// <summary>
    /// Creates a new course. Restricted to Admins.
    /// </summary>
    /// <param name="newCourse">The course creation payload. See <see cref="CreateCourseDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the newly created <see cref="CourseDto"/>.</returns>
    /// <response code="201">
    /// Course created successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseCreatedSuccess</c> – The course was persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">The request payload failed validation.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<CourseDto>>> CreateCourse([FromBody] CreateCourseDto newCourse)
    {
        var course = new Course { Name = newCourse.Name, Code = newCourse.Code };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var createdCourseDto = new CourseDto { Id = course.Id, Code = course.Code, Name = course.Name };

        return CreatedAtAction(nameof(GetCourse), new { courseId = createdCourseDto.Id },
            new ServiceResponse<CourseDto> { Data = createdCourseDto, Message = "CourseCreatedSuccess", StatusCode = 201 });
    }

    /// <summary>
    /// Updates an existing course's details. Restricted to Admins.
    /// </summary>
    /// <param name="courseId">The ID of the course to update.</param>
    /// <param name="updatedCourse">The updated course payload. See <see cref="CreateCourseDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Course updated successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseUpdatedSuccess</c> – Changes were persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">The request payload failed validation.</response>
    /// <response code="404">
    /// The course was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseNotFound</c> – No course exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{courseId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateCourse(int courseId, [FromBody] CreateCourseDto updatedCourse)
    {
        var existingCourse = await context.Courses.FindAsync(courseId);
        if (existingCourse == null)
            return NotFound(new ErrorServiceResponse { Message = "CourseNotFound", StatusCode = 404 });

        existingCourse.Code = updatedCourse.Code;
        existingCourse.Name = updatedCourse.Name;
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "CourseUpdatedSuccess" });
    }

    /// <summary>
    /// Permanently deletes a course. Restricted to Admins.
    /// </summary>
    /// <param name="courseId">The ID of the course to delete.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Course deleted successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseDeletedSuccess</c> – The course was removed.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The course was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseNotFound</c> – No course exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{courseId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteCourse(int courseId)
    {
        var existingCourse = await context.Courses.FindAsync(courseId);
        if (existingCourse == null)
            return NotFound(new ErrorServiceResponse { Message = "CourseNotFound", StatusCode = 404 });

        context.Courses.Remove(existingCourse);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "CourseDeletedSuccess" });
    }
}