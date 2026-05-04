using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;

namespace EExamSystem.Api.Controllers;

[ApiController]
[Route("api/v1/courses")] 
public class CoursesController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all courses currently available in the database.
    /// </summary>
    /// <returns>A collection of CourseDto objects.</returns>
    /// <response code="200">Returns the list successfully.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
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
}