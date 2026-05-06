using EExamSystem.Shared.DTOs.Assignments;
using EExamSystem.Shared.DTOs.Exams;
using EExamSystem.Shared.DTOs.Sections;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Defines the contract for assigning exams to sections and verifying course constraints.
/// </summary>
public interface IExamAssignmentService
{
    /// <summary>
    /// Links an exam to a section after validating they belong to the same course.
    /// </summary>
    Task<AssignmentResultDto<bool>> AssignExamToSectionAsync(int examId, int sectionId);

    /// <summary>
    /// Removes an exam assignment from a specific section.
    /// </summary>
    Task<AssignmentResultDto<bool>> RevokeExamFromSectionAsync(int examId, int sectionId);

    /// <summary>
    /// Retrieves all sections that have been assigned a specific exam.
    /// </summary>
    Task<AssignmentResultDto<IEnumerable<SectionDto>>> GetSectionsForExamAsync(int examId);

    /// <summary>
    /// Retrieves all active exams assigned to a specific section.
    /// </summary>
    Task<AssignmentResultDto<IEnumerable<ExamDto>>> GetExamsForSectionAsync(int sectionId);
}