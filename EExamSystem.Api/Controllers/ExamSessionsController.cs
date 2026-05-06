using System.Security.Claims;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the real-time execution of exams, including starting timers and recording answers.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1")]
public class ExamSessionsController(IExamExecutionService executionService) : ControllerBase
{
    /// <summary>
    /// Starts a new exam session or resumes an existing one for the authenticated student.
    /// </summary>
    /// <param name="examId">The ID of the exam to start.</param>
    /// <returns>An ExamSessionDto containing the session ID and timer details.</returns>
    /// <response code="200">Successfully started or resumed the exam session.</response>
    /// <response code="400">If the exam is not currently active or has already been completed.</response>
    /// <response code="401">If the user is not authenticated or token is invalid.</response>
    /// <response code="403">If the student is not on the enrollment roster for this exam.</response>
    /// <response code="404">If the specified exam does not exist.</response>
    [HttpPost("exams/{examId}/sessions")]
    [ProducesResponseType(typeof(ServiceResponse<ExamSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ExamSessionDto>>> StartExam(int examId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) 
            return Unauthorized(new ServiceResponse<ExamSessionDto> { Success = false, Message = "Invalid user identity.", StatusCode = 401 });

        var response = await executionService.StartSessionAsync(examId, userId);
        if (!response.Success) return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Records a student's answer to a specific question during an active session.
    /// </summary>
    /// <param name="sessionId">The ID of the active exam session.</param>
    /// <param name="payload">The question ID and the selected option ID.</param>
    /// <response code="200">Successfully auto-saved the answer.</response>
    /// <response code="400">If the provided option ID does not belong to the question.</response>
    /// <response code="401">If the user is not authenticated or token is invalid.</response>
    /// <response code="404">If the session is invalid, expired, or already submitted.</response>
    [HttpPut("sessions/{sessionId}/answers")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> SubmitAnswer(int sessionId, [FromBody] SubmitAnswerDto payload)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) 
            return Unauthorized(new ServiceResponse<bool> { Success = false, Message = "Invalid user identity.", StatusCode = 401 });

        var response = await executionService.SubmitAnswerAsync(sessionId, userId, payload);
        if (!response.Success) return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Finalizes an active exam session, calculates the grade, and locks the test.
    /// </summary>
    /// <param name="sessionId">The ID of the session to submit.</param>
    /// <returns>An ExamResultDto containing the final score.</returns>
    /// <response code="200">Successfully graded and submitted the exam.</response>
    /// <response code="401">If the user is not authenticated or token is invalid.</response>
    /// <response code="404">If the session is invalid or already submitted.</response>
    [HttpPost("sessions/{sessionId}/submit")]
    [ProducesResponseType(typeof(ServiceResponse<ExamResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<ExamResultDto>>> SubmitExam(int sessionId)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId)) 
            return Unauthorized(new ServiceResponse<ExamResultDto> { Success = false, Message = "Invalid user identity.", StatusCode = 401 });

        var response = await executionService.GradeAndSubmitExamAsync(sessionId, userId);
        if (!response.Success) return StatusCode(response.StatusCode, response);

        return Ok(response);
    }
}