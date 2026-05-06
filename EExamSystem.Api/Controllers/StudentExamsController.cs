using System.Security.Claims;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.StudentDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Provides the student dashboard and pre-test lobby endpoints. All routes are restricted to the Student role.
/// </summary>
[Authorize(Roles = "Student")]
[ApiController]
[Route("api/v1/[controller]")]
public class StudentExamsController : ControllerBase
{
    private readonly IStudentExamsService _studentExamsService;

    public StudentExamsController(IStudentExamsService studentExamsService)
    {
        _studentExamsService = studentExamsService;
    }

    /// <summary>
    /// Retrieves all active and upcoming exams available to the currently authenticated student.
    /// </summary>
    /// <remarks>
    /// Only exams assigned to sections the student is enrolled in, and whose window has not yet closed, are returned.
    /// The student identity is resolved from the JWT bearer token — no ID parameter is required.
    /// </remarks>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="ActiveExamDto"/>.</returns>
    /// <response code="200">Successfully retrieved the student's active exam list.</response>
    /// <response code="401">The request is missing a valid JWT bearer token.</response>
    /// <response code="404">
    /// The student profile was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentNotFound</c> – No profile exists for the authenticated user ID.</item>
    /// </list>
    /// </response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ActiveExamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveExams()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _studentExamsService.GetActiveExamsAsync(studentId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<IEnumerable<ActiveExamDto>> { Data = result.Data });
    }

    /// <summary>
    /// Retrieves the completed exam history and final grades for the currently authenticated student.
    /// </summary>
    /// <remarks>
    /// Only submitted sessions are included. Active or abandoned sessions do not appear in history.
    /// The student identity is resolved from the JWT bearer token — no ID parameter is required.
    /// </remarks>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="ExamHistoryDto"/>.</returns>
    /// <response code="200">Successfully retrieved the student's exam history.</response>
    /// <response code="401">The request is missing a valid JWT bearer token.</response>
    /// <response code="404">
    /// The student profile was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>StudentNotFound</c> – No profile exists for the authenticated user ID.</item>
    /// </list>
    /// </response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ExamHistoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExamHistory()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _studentExamsService.GetExamHistoryAsync(studentId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<IEnumerable<ExamHistoryDto>> { Data = result.Data });
    }

    /// <summary>
    /// Retrieves the pre-test lobby details for a specific exam before the student begins.
    /// </summary>
    /// <remarks>
    /// Returns metadata such as exam title, duration, and scheduled window so the student can review the rules before starting.
    /// A 404 is intentionally returned for both not-found and unauthorized access cases to prevent probing for valid exam IDs.
    /// </remarks>
    /// <param name="examId">The ID of the exam to inspect.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing an <see cref="ExamPreTestDetailsDto"/>.</returns>
    /// <response code="200">Successfully retrieved the exam lobby details.</response>
    /// <response code="401">The request is missing a valid JWT bearer token.</response>
    /// <response code="404">
    /// The exam is inaccessible. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID, it has ended, or the student is not enrolled in an assigned section.</item>
    /// </list>
    /// </response>
    [HttpGet("{examId}/details")]
    [ProducesResponseType(typeof(ServiceResponse<ExamPreTestDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExamLobbyDetails(int examId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _studentExamsService.GetExamDetailsAsync(studentId, examId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<ExamPreTestDetailsDto> { Data = result.Data });
    }
}