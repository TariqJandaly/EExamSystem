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
    /// <response code="200">If the student was successfully enrolled.</response>
    /// <response code="400">If the student is already enrolled in this section.</response>
    /// <response code="404">If the section or student user does not exist.</response>
    [Authorize(Roles = "INSTRUCTOR,ADMIN")]
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
    /// <response code="200">If the student was successfully unenrolled.</response>
    /// <response code="404">If the section or student membership was not found.</response>
    [Authorize(Roles = "INSTRUCTOR,ADMIN")]
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
    /// Retrieves all course sections associated with a specific student.
    /// </summary>
    /// <param name="studentId">The unique ID of the student.</param>
    /// <response code="200">Returns the list of sections the student is enrolled in.</response>
    /// <response code="404">If the student record is not found.</response>
    [Authorize(Roles = "INSTRUCTOR,ADMIN")]
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