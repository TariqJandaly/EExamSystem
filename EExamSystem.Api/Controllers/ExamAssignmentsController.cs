using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Exams;
using EExamSystem.Shared.DTOs.Sections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the scheduling and assignment of exams to specific course sections.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ExamAssignmentsController : ControllerBase
{
    private readonly IExamAssignmentService _assignmentService;

    public ExamAssignmentsController(IExamAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    /// <summary>
    /// Assigns a specific exam to a section.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="sectionId">The ID of the section.</param>
    /// <response code="200">If the exam was successfully assigned.</response>
    /// <response code="400">If the exam and section belong to different courses, or if already assigned.</response>
    /// <response code="404">If the exam or section does not exist.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("exams/{examId}/sections/{sectionId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignExam(int examId, int sectionId)
    {
        var result = await _assignmentService.AssignExamToSectionAsync(examId, sectionId);

        if (!result.IsSuccess)
        {
            if (result.Message == "CourseMismatch" || result.Message == "ExamAlreadyAssigned")
                return BadRequest(new ErrorServiceResponse { Message = result.Message, StatusCode = 400 });

            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });
        }

        return Ok(new ServiceResponse<bool> { Data = true, Message = result.Message });
    }

    /// <summary>
    /// Revokes an exam assignment from a section.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="sectionId">The ID of the section.</param>
    /// <response code="200">If the assignment was successfully revoked.</response>
    /// <response code="404">If the exam, section, or specific assignment was not found.</response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("exams/{examId}/sections/{sectionId}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeAssignment(int examId, int sectionId)
    {
        var result = await _assignmentService.RevokeExamFromSectionAsync(examId, sectionId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<bool> { Data = true, Message = result.Message });
    }

    /// <summary>
    /// Retrieves all sections that have been assigned a specific exam.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <response code="200">Returns the list of assigned sections.</response>
    /// <response code="404">If the exam is not found.</response>
    [HttpGet("exams/{examId}/sections")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<SectionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssignedSections(int examId)
    {
        var result = await _assignmentService.GetSectionsForExamAsync(examId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<IEnumerable<SectionDto>> { Data = result.Data });
    }

    /// <summary>
    /// Retrieves all exams scheduled for a specific section.
    /// </summary>
    /// <param name="sectionId">The ID of the section.</param>
    /// <response code="200">Returns the list of assigned exams.</response>
    /// <response code="404">If the section is not found.</response>
    [HttpGet("sections/{sectionId}/exams")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<ExamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssignedExams(int sectionId)
    {
        var result = await _assignmentService.GetExamsForSectionAsync(sectionId);

        if (!result.IsSuccess)
            return NotFound(new ErrorServiceResponse { Message = result.Message, StatusCode = 404 });

        return Ok(new ServiceResponse<IEnumerable<ExamDto>> { Data = result.Data });
    }
}