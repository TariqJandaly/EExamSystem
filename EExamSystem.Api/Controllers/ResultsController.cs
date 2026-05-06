using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the retrieval of exam grades, scores, and class reports.
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
    /// Retrieves the grading report for a specific section taking a specific exam.
    /// </summary>
    /// <remarks>
    /// The report includes all students enrolled in the section. Students who have not taken the exam will appear with null scores.
    /// </remarks>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="sectionId">The ID of the section.</param>
    /// <returns>A comprehensive grading report payload.</returns>
    /// <response code="200">Successfully retrieved the grading report.</response>
    /// <response code="400">If the requested section is not assigned to the requested exam.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user lacks the Instructor or Admin role.</response>
    /// <response code="404">If the exam cannot be found.</response>
    /// <response code="500">If an unexpected server error occurs.</response>
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

            // Fallback for infrastructure/unhandled failures
            return StatusCode(500, new ErrorServiceResponse { Message = result.Message ?? "InternalServerError", StatusCode = 500 });
        }

        return Ok(new ServiceResponse<SectionResultsDto> { Data = result.Data });
    }
}