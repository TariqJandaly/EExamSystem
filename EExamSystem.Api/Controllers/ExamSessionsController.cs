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
    public ExamSessionsController(IExamSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Starts a new exam session or resumes an existing unsubmitted one.
    /// </summary>
    /// <remarks>
    /// This endpoint strips correct answers from the response payload to prevent cheating.
    /// If a prior unsubmitted session exists, it is resumed and previously saved answers are rehydrated into the payload.
    /// </remarks>
    /// <param name="examId">The unique identifier of the exam to start or resume.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing an <see cref="ActiveSessionDto"/> with the safe exam payload and submission deadline.</returns>
    /// <response code="200">Session started or resumed. <c>Data</c> contains the exam questions and deadline.</response>
    /// <response code="400">
    /// The session cannot be started. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamAlreadySubmitted</c> – This exam session was already submitted and locked.</item>
    /// </list>
    /// </response>
    /// <response code="401">The student is not authenticated.</response>
    /// <response code="404">
    /// The exam is inaccessible. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamUnavailableOrUnauthorized</c> – The exam does not exist, has expired, or the student is not enrolled in an assigned section.</item>
    /// </list>
    /// </response>
    /// <response code="500">
    /// An unexpected infrastructure or operational error occurred. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InternalServerError</c> – A fallback for unhandled failures.</item>
    /// </list>
    /// </response>
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

            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<ActiveSessionDto> { Data = result.Data });
    }

    /// <summary>
    /// Auto-saves a student's answer during an active exam session.
    /// </summary>
    /// <remarks>
    /// Performs an atomic upsert — if the student previously answered this question, the record is updated rather than duplicated.
    /// </remarks>
    /// <param name="sessionId">The unique identifier of the active exam session.</param>
    /// <param name="answerDto">The answer payload. See <see cref="SubmitAnswerDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> when the answer is saved.</returns>
    /// <response code="200">Answer saved or updated successfully.</response>
    /// <response code="400">
    /// The answer was rejected. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SessionLocked</c> – The session has already been submitted.</item>
    ///   <item><c>TimeExpired</c> – The exam duration has elapsed.</item>
    ///   <item><c>InvalidOption</c> – The selected option ID does not belong to the given question.</item>
    ///   <item><c>InvalidQuestionForExam</c> – The question does not belong to this exam's covered chapters.</item>
    /// </list>
    /// </response>
    /// <response code="401">The student is not authenticated.</response>
    /// <response code="404">
    /// The session was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SessionNotFound</c> – No active session exists with the given ID for this student.</item>
    /// </list>
    /// </response>
    /// <response code="500">
    /// An unexpected database or server error occurred. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InternalServerError</c> – A fallback for unhandled failures.</item>
    /// </list>
    /// </response>
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
            if (result.Message == "SessionLocked" || result.Message == "TimeExpired" ||
                result.Message == "InvalidOption" || result.Message == "InvalidQuestionForExam")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            if (result.Message == "SessionNotFound")
                return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<bool> { Data = result.Data });
    }

    /// <summary>
    /// Submits and locks the exam session, then triggers auto-grading.
    /// </summary>
    /// <remarks>
    /// Once submitted, the session is permanently locked. No further answer changes are accepted.
    /// Grading is calculated immediately based on the saved answers at the time of submission.
    /// </remarks>
    /// <param name="sessionId">The unique identifier of the session to submit.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the final <c>decimal</c> score.</returns>
    /// <response code="200">Exam submitted and graded. <c>Data</c> contains the final score.</response>
    /// <response code="400">
    /// Submission was rejected. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SessionAlreadySubmitted</c> – This session was already submitted and cannot be re-graded.</item>
    /// </list>
    /// </response>
    /// <response code="401">The student is not authenticated.</response>
    /// <response code="404">
    /// The session was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SessionNotFound</c> – No active session exists with the given ID for this student.</item>
    /// </list>
    /// </response>
    /// <response code="500">
    /// An unexpected error occurred during grading. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InternalServerError</c> – A fallback for unhandled failures.</item>
    /// </list>
    /// </response>
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