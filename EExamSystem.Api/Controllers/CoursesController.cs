using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EExamSystem.Api.Data;
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
    /// Retrieves a list of all courses currently available in the database.
    /// </summary>
    /// <returns>A collection of CourseDto objects.</returns>
    /// <response code="200">Returns the list successfully.</response>
    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await context.Courses
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name
            })
            .ToListAsync();

        return Ok(courses); 
    }
    
    /// <summary>
    /// Retrieves the details of a specific course by its ID.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>A CourseDto containing the course details.</returns>
    /// <response code="200">Returns the requested course.</response>
    /// <response code="404">If the course does not exist.</response>
    [HttpGet("{courseId}")] 
    public async Task<IActionResult> GetCourse(int courseId)
    {
        var course = await context.Courses
            .Where(c => c.Id == courseId)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name
            })
            .FirstOrDefaultAsync();

        if (course == null)
        {
            return NotFound($"Course with ID {courseId} was not found.");
        }

        return Ok(course);
    }
    
    /// <summary>
    /// Creates a brand-new course in the database.
    /// </summary>
    /// <param name="newCourse">The details of the course to create.</param>
    /// <returns>The newly created CourseDto.</returns>
    /// <response code="201">Returns the newly created course successfully.</response>
    /// <response code="400">If the provided data is invalid.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto newCourse)
    {
        var course = new Course
        {
            Name = newCourse.Name,
            Code = newCourse.Code
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync(); 

        var createdCourseDto = new CourseDto
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name
        };

        return CreatedAtAction(
            nameof(GetCourse),
            new { courseId = createdCourseDto.Id },
            createdCourseDto
        );
    }

    /// <summary>
    /// Updates an existing course's details.
    /// </summary>
    /// <param name="courseId">The ID of the course to update.</param>
    /// <param name="updatedCourse">The updated course data.</param>
    /// <response code="204">Successfully updated the course.</response>
    /// <response code="400">If the ID in the URL does not match the ID in the body.</response>
    /// <response code="404">If the course does not exist.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{courseId}")]
    public async Task<ActionResult> UpdateCourse(int courseId, [FromBody] CreateCourseDto updatedCourse)
    {
        var existingCourse = await context.Courses.FindAsync(courseId);
        
        if (existingCourse == null)
        {
            return NotFound($"Course with ID {courseId} was not found.");
        }
        existingCourse.Code = updatedCourse.Code;
        existingCourse.Name = updatedCourse.Name;

        await context.SaveChangesAsync();

        return NoContent();
    }
    
    /// <summary>
    /// Deletes a specific course from the system.
    /// </summary>
    /// <param name="courseId">The ID of the course to delete.</param>
    /// <response code="204">Successfully deleted the course.</response>
    /// <response code="404">If the course does not exist.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{courseId}")]
    public async Task<ActionResult> DeleteCourse(int courseId)
    {
        var existingCourse = await context.Courses.FindAsync(courseId);
        
        if (existingCourse == null)
        {
            return NotFound($"Course with ID {courseId} was not found.");
        }

        context.Courses.Remove(existingCourse);

        await context.SaveChangesAsync();

        return NoContent();
    }
}