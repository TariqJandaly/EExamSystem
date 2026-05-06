using System.Security.Claims;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.StudentDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Provides the dashboard and pre-test endpoints exclusively for students.
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
    /// Retrieves all active and upcoming exams assigned to the currently logged-in student.
    /// </summary>
    /// <response code="200">Returns the list of active exams.</response>
    /// <response code="401">If the student is not authenticated.</response>
    /// <response code="404">If the student profile cannot be found.</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ActiveExamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveExams()
    {
        // Securely extract the logged-in user's ID from the JWT token
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _studentExamsService.GetActiveExamsAsync(studentId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<IEnumerable<ActiveExamDto>> { Data = result.Data });
    }

    /// <summary>
    /// Retrieves the exam history and grades for the currently logged-in student.
    /// </summary>
    /// <response code="200">Returns the list of completed exams.</response>
    /// <response code="401">If the student is not authenticated.</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ExamHistoryDto>>), StatusCodes.Status200OK)]
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
    /// Retrieves the lobby details for a specific exam before the student begins.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <response code="200">Returns the exam lobby details.</response>
    /// <response code="403">If the student is not authorized to view this exam.</response>
    /// <response code="404">If the exam is not found.</response>
    [HttpGet("{examId}/details")]
    [ProducesResponseType(typeof(ServiceResponse<ExamPreTestDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExamLobbyDetails(int examId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _studentExamsService.GetExamDetailsAsync(studentId, examId);

        if (!result.IsSuccess)
        {
            // Using 404 for both Not Found and Unauthorized access to prevent probing
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });
        }

        return Ok(new ServiceResponse<ExamPreTestDetailsDto> { Data = result.Data });
    }
}