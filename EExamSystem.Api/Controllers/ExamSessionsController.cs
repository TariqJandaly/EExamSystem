using System.Security.Claims;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the active test engine for students, including starting exams, auto-saving answers, and submitting sessions for grading.
/// </summary>
[Authorize(Roles = "Student")]
[ApiController]
[Route("api/v1/[controller]")]
public class ExamSessionsController : ControllerBase
{
    private readonly IExamSessionService _sessionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExamSessionsController"/> class.
    /// </summary>
    /// <param name="sessionService">The service handling exam session business logic.</param>
    public ExamSessionsController(IExamSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Starts a new exam session or resumes an ongoing, unsubmitted session.
    /// </summary>
    /// <remarks>
    /// This endpoint securely strips correct answers from the payload to prevent cheating. 
    /// If resuming, it will also rehydrate the payload with the student's previously saved answers.
    /// </remarks>
    /// <param name="examId">The unique identifier of the exam to start or resume.</param>
    /// <returns>An <see cref="ActiveSessionDto"/> containing the safe exam payload and deadline.</returns>
    /// <response code="200">Successfully started or resumed the exam session.</response>
    /// <response code="400">If the exam has already been submitted and cannot be resumed.</response>
    /// <response code="401">If the student is not authenticated.</response>
    /// <response code="404">If the exam is unavailable, expired, or the student is not authorized to take it.</response>
    /// <response code="500">If an unexpected operational or infrastructure error occurs.</response>
    [HttpPost("start/{examId}")]
    [ProducesResponseType(typeof(ServiceResponse<ActiveSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartSession(int examId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _sessionService.StartOrResumeSessionAsync(studentId, examId);

        if (!result.IsSuccess)
        {
            if (result.Message == "ExamAlreadySubmitted")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            if (result.Message == "ExamUnavailableOrUnauthorized")
                return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

            // Graceful fallback for unexpected operational/infrastructure errors
            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<ActiveSessionDto> { Data = result.Data });
    }

    /// <summary>
    /// Auto-saves a student's answer choice during an active exam session.
    /// </summary>
    /// <remarks>
    /// This endpoint performs an atomic upsert. If the student changes their mind and selects a different option, the existing answer record is updated.
    /// </remarks>
    /// <param name="sessionId">The unique identifier of the active exam session.</param>
    /// <param name="answerDto">The payload containing the question ID and the selected option ID.</param>
    /// <returns>A boolean indicating if the save operation was successful.</returns>
    /// <response code="200">The answer was successfully saved or updated.</response>
    /// <response code="400">If the session is locked, time has expired, or the provided question/option is invalid.</response>
    /// <response code="401">If the student is not authenticated.</response>
    /// <response code="404">If the active session cannot be found.</response>
    /// <response code="500">If an unexpected database or server error occurs.</response>
    [HttpPut("{sessionId}/answers")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveAnswer(int sessionId, [FromBody] SubmitAnswerDto answerDto)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _sessionService.SaveAnswerAsync(studentId, sessionId, answerDto);

        if (!result.IsSuccess)
        {
            if (result.Message == "SessionLocked" || result.Message == "TimeExpired" || result.Message == "InvalidOption" || result.Message == "InvalidQuestionForExam")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            if (result.Message == "SessionNotFound")
                return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<bool> { Data = result.Data });
    }

    /// <summary>
    /// Submits the exam, locks the session to prevent further changes, and triggers the auto-grading process.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session to submit.</param>
    /// <returns>The final calculated score based on the saved answers.</returns>
    /// <response code="200">The exam was successfully submitted and graded.</response>
    /// <response code="400">If the session has already been submitted and locked.</response>
    /// <response code="401">If the student is not authenticated.</response>
    /// <response code="404">If the active session cannot be found.</response>
    /// <response code="500">If an unexpected operational error occurs during grading.</response>
    [HttpPost("{sessionId}/submit")]
    [ProducesResponseType(typeof(ServiceResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitExam(int sessionId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _sessionService.SubmitAndGradeSessionAsync(studentId, sessionId);

        if (!result.IsSuccess)
        {
            if (result.Message == "SessionAlreadySubmitted")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            if (result.Message == "SessionNotFound")
                return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<decimal> { Data = result.Data, Message = result.Message });
    }
}