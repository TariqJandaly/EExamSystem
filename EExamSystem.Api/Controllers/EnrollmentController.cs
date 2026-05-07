using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Sections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the student enrollment lifecycle for course sections.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Enrolls a student into a specific section.
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
    [HttpPost("sections/{sectionId}/students/{studentId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Enroll(int sectionId, string studentId)
    {
        var result = await _enrollmentService.EnrollStudentAsync(sectionId, studentId);

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
    /// <param name="studentId">The GUID of the student user.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Student unenrolled successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentUnenrolledSuccess</c> – The student was removed from the section.</item>
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
    [HttpDelete("sections/{sectionId}/students/{studentId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unenroll(int sectionId, string studentId)
    {
        var result = await _enrollmentService.UnenrollStudentAsync(sectionId, studentId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<bool> { Data = true, Message = result.Message });
    }

    /// <summary>
    /// Retrieves all course sections a specific student is enrolled in.
    /// </summary>
    /// <param name="studentId">The GUID of the student.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="SectionDto"/>.</returns>
    /// <response code="200">Successfully retrieved the student's section enrollments.</response>
    /// <response code="404">
    /// The student was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentNotFound</c> – No user account exists for the given student ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpGet("students/{studentId}/sections")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<SectionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentEnrollments(string studentId)
    {
        var result = await _enrollmentService.GetStudentSectionsAsync(studentId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<IEnumerable<SectionDto>> { Data = result.Data });
    }
}