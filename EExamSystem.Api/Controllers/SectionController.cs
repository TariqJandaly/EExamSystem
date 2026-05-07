using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Sections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages course sections and student enrollment within them.
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
    /// Retrieves all sections belonging to a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="SectionDto"/>.</returns>
    /// <response code="200">Successfully retrieved the section list.</response>
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
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the matching <see cref="SectionDto"/>.</returns>
    /// <response code="200">Successfully retrieved the section.</response>
    /// <response code="404">
    /// The section was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    /// </list>
    /// </response>
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
    /// Creates a new section for a course. Restricted to Instructors and Admins.
    /// </summary>
    /// <param name="model">The section creation payload. See <see cref="CreateSectionDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the newly created <see cref="SectionDto"/>.</returns>
    /// <response code="201">
    /// Section created successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionCreatedSuccess</c> – The section was persisted under the given course.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The parent course was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseNotFound</c> – No course exists with the ID specified in the payload.</item>
    /// </list>
    /// </response>
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
    /// Enrolls a student into a section. Restricted to Instructors and Admins.
    /// </summary>
    /// <param name="sectionId">The ID of the target section.</param>
    /// <param name="studentId">The GUID of the student user.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Student enrolled successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentEnrolledSuccess</c> – The student was added to the section.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// Enrollment rejected. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentAlreadyEnrolled</c> – The student is already a member of this section.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    ///   <item><c>StudentNotFound</c> – No user account exists for the given student ID.</item>
    /// </list>
    /// </response>
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
    /// Removes a student from a section. Restricted to Instructors and Admins.
    /// </summary>
    /// <param name="sectionId">The ID of the section.</param>
    /// <param name="studentId">The GUID of the student to remove.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Student removed successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentRemovedSuccess</c> – The student was unenrolled from the section.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    ///   <item><c>EnrollmentNotFound</c> – The student is not enrolled in this section.</item>
    /// </list>
    /// </response>
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
    /// Permanently deletes a section. Restricted to Instructors and Admins.
    /// </summary>
    /// <param name="id">The ID of the section to delete.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Section deleted successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionDeletedSuccess</c> – The section was removed.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The section was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    /// </list>
    /// </response>
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