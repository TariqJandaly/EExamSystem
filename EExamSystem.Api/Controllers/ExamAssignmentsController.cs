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
    /// Assigns an exam to a section, making it visible and accessible to enrolled students.
    /// </summary>
    /// <param name="examId">The ID of the exam to assign.</param>
    /// <param name="sectionId">The ID of the target section.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Exam assigned successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamAssignedSuccess</c> – The exam was linked to the section.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// Assignment rejected. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>CourseMismatch</c> – The exam and section belong to different courses.</item>
    ///   <item><c>ExamAlreadyAssigned</c> – This exam is already assigned to the given section.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    /// </list>
    /// </response>
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
    /// Revokes an exam assignment from a section, preventing enrolled students from accessing it.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="sectionId">The ID of the section.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Assignment revoked successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>AssignmentRevokedSuccess</c> – The exam was unlinked from the section.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// A required resource was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    ///   <item><c>AssignmentNotFound</c> – This exam is not currently assigned to the given section.</item>
    /// </list>
    /// </response>
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
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="SectionDto"/>.</returns>
    /// <response code="200">Successfully retrieved the list of assigned sections.</response>
    /// <response code="404">
    /// The exam was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ExamNotFound</c> – No exam exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
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
    /// Retrieves all exams currently assigned to a specific section.
    /// </summary>
    /// <param name="sectionId">The ID of the section.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="ExamDto"/>.</returns>
    /// <response code="200">Successfully retrieved the list of exams for the section.</response>
    /// <response code="404">
    /// The section was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>SectionNotFound</c> – No section exists with the given ID.</item>
    /// </list>
    /// </response>
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