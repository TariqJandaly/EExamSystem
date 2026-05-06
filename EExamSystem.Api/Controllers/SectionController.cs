using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Sections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages course sections and administrative student enrollment operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly ISectionService _sectionService;

    public SectionsController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    /// <summary>
    /// Retrieves a collection of sections for a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <response code="200">Returns the list of sections successfully.</response>
    [HttpGet("course/{courseId}")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<SectionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<SectionDto>>>> GetSections(int courseId)
    {
        var result = await _sectionService.GetSectionsByCourseAsync(courseId);
        return Ok(new ServiceResponse<IEnumerable<SectionDto>> { Data = result.Data });
    }

    /// <summary>
    /// Retrieves the details of a specific section by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the section.</param>
    /// <response code="200">Returns the requested section data.</response>
    /// <response code="404">If the section does not exist.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<SectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<SectionDto>>> GetSection(int id)
    {
        var result = await _sectionService.GetSectionByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<SectionDto> { Data = result.Data });
    }

    /// <summary>
    /// Creates a new section for a course. Resticted to Instructors and Admins.
    /// </summary>
    /// <param name="model">The section creation data.</param>
    /// <response code="201">Returns the newly created section.</response>
    /// <response code="404">If the parent course is not found.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse<SectionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<SectionDto>>> CreateSection([FromBody] CreateSectionDto model)
    {
        var result = await _sectionService.CreateSectionAsync(model);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return CreatedAtAction(nameof(GetSection), new { id = result.Data?.Id }, 
            new ServiceResponse<SectionDto> { Data = result.Data, Message = result.Message, StatusCode = 201 });
    }

    /// <summary>
    /// Enrolls a student into a section.
    /// </summary>
    /// <param name="sectionId">The ID of the target section.</param>
    /// <param name="studentId">The ID of the student user.</param>
    /// <response code="200">If the student was successfully enrolled.</response>
    /// <response code="400">If the student is already a member of the section.</response>
    /// <response code="404">If the section or student was not found.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("{sectionId}/students/{studentId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> EnrollStudent(int sectionId, string studentId)
    {
        var result = await _sectionService.AddStudentToSectionAsync(sectionId, studentId);

        if (!result.IsSuccess)
        {
            if (result.Message == "StudentAlreadyEnrolled")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });
        }

        return Ok(new ServiceResponse<bool> { Data = true, Message = result.Message });
    }

    /// <summary>
    /// Removes a student from a section.
    /// </summary>
    /// <param name="sectionId">The ID of the section.</param>
    /// <param name="studentId">The ID of the student to remove.</param>
    /// <response code="200">If removal was successful.</response>
    /// <response code="404">If section or enrollment was not found.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("{sectionId}/students/{studentId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> RemoveStudent(int sectionId, string studentId)
    {
        var result = await _sectionService.RemoveStudentFromSectionAsync(sectionId, studentId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<bool> { Data = true, Message = result.Message });
    }

    /// <summary>
    /// Deletes an existing section.
    /// </summary>
    /// <param name="id">The ID of the section to delete.</param>
    /// <response code="200">If deletion was successful.</response>
    /// <response code="404">If the section was not found.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteSection(int id)
    {
        var result = await _sectionService.DeleteSectionAsync(id);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<bool> { Data = true, Message = result.Message });
    }
}