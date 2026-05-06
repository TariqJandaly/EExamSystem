using System.Security.Claims;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the active test engine: starting exams, saving progress, and submitting for a grade.
/// </summary>
[Authorize(Roles = "Student")]
[ApiController]
[Route("api/v1/[controller]")]
public class ExamSessionsController : ControllerBase
{
    private readonly IExamSessionService _sessionService;

    public ExamSessionsController(IExamSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Starts a new exam session or resumes an ongoing one. Returns the questions with correct answers stripped out.
    /// </summary>
    /// <param name="examId">The ID of the exam to start.</param>
    /// <response code="200">Returns the active session payload.</response>
    /// <response code="400">If the exam has already been completed and submitted.</response>
    /// <response code="404">If the exam is unavailable, expired, or the student is unauthorized.</response>
    [HttpPost("start/{examId}")]
    [ProducesResponseType(typeof(ServiceResponse<ActiveSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartSession(int examId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _sessionService.StartOrResumeSessionAsync(studentId, examId);

        if (!result.IsSuccess)
        {
            if (result.Message == "ExamAlreadySubmitted")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });
        }

        return Ok(new ServiceResponse<ActiveSessionDto> { Data = result.Data });
    }

    /// <summary>
    /// Auto-saves a student's answer choice during an active exam.
    /// </summary>
    /// <param name="sessionId">The active session ID.</param>
    /// <param name="answerDto">The question ID and selected option ID.</param>
    [HttpPut("{sessionId}/answers")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveAnswer(int sessionId, [FromBody] SubmitAnswerDto answerDto)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _sessionService.SaveAnswerAsync(studentId, sessionId, answerDto);

        if (!result.IsSuccess)
        {
            if (result.Message == "SessionLocked" || result.Message == "TimeExpired" || result.Message == "InvalidOption")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });
        }

        return Ok(new ServiceResponse<bool> { Data = true });
    }

    /// <summary>
    /// Submits the exam, locks the session, and triggers auto-grading.
    /// </summary>
    /// <param name="sessionId">The ID of the session to submit.</param>
    /// <response code="200">Returns the final calculated score.</response>
    [HttpPost("{sessionId}/submit")]
    [ProducesResponseType(typeof(ServiceResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitExam(int sessionId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _sessionService.SubmitAndGradeSessionAsync(studentId, sessionId);

        if (!result.IsSuccess)
        {
            if (result.Message == "SessionAlreadySubmitted")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });
        }

        return Ok(new ServiceResponse<decimal> { Data = result.Data, Message = result.Message });
    }
}