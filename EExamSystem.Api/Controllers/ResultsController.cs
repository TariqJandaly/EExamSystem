using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the retrieval of exam grades, scores, and class-level performance reports.
/// </summary>
[Authorize(Roles = "Instructor,Admin")]
[ApiController]
[Route("api/v1/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly IResultsService _resultsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultsController"/> class.
    /// </summary>
    public ResultsController(IResultsService resultsService)
    {
        _resultsService = resultsService;
    }

    /// <summary>
    /// Retrieves the full grading report for a section taking a specific exam.
    /// </summary>
    /// <remarks>
    /// All students enrolled in the section are included in the report. Students who have not yet taken the exam will appear with <c>null</c> score fields.
    /// </remarks>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="sectionId">The ID of the section.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a <see cref="SectionResultsDto"/> with per-student scores.</returns>
    /// <response code="200">Successfully retrieved the grading report. <c>Data</c> contains the full section results.</response>
    /// <response code="400">
    /// The request is invalid. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionNotAssignedToExam</c> – The requested section has not been assigned to this exam.</item>
    /// </list>
    /// </response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks the Instructor or Admin role.</response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    /// </list>
    /// </response>
    /// <response code="500">
    /// An unexpected server error occurred. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InternalServerError</c> – A fallback for unhandled infrastructure failures.</item>
    /// </list>
    /// </response>
    [HttpGet("exams/{examId}/sections/{sectionId}")]
    [ProducesResponseType(typeof(ServiceResponse<SectionResultsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSectionResults(int examId, int sectionId)
    {
        var result = await _resultsService.GetSectionResultsAsync(examId, sectionId);

        if (!result.IsSuccess)
        {
            if (result.Message == "SectionNotAssignedToExam")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            if (result.Message == "ExamNotFound")
                return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<SectionResultsDto> { Data = result.Data });
    }
}