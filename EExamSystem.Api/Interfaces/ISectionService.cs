using EExamSystem.Shared.DTOs.Sections;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Defines the contract for business logic operations related to course sections and student enrollments.
/// </summary>
public interface ISectionService
{
    /// <summary>
    /// Retrieves all sections belonging to a specific course.
    /// </summary>
    Task<SectionResultDto<IEnumerable<SectionDto>>> GetSectionsByCourseAsync(int courseId);

    /// <summary>
    /// Retrieves a single section by its unique ID.
    /// </summary>
    Task<SectionResultDto<SectionDto>> GetSectionByIdAsync(int id);

    /// <summary>
    /// Validates and creates a new section within an existing course.
    /// </summary>
    Task<SectionResultDto<SectionDto>> CreateSectionAsync(CreateSectionDto model);

    /// <summary>
    /// Enrolls a student into a specific section after verifying existence and duplicate status.
    /// </summary>
    Task<SectionResultDto<bool>> AddStudentToSectionAsync(int sectionId, string studentId);

    /// <summary>
    /// Removes a student from a section's enrollment list.
    /// </summary>
    Task<SectionResultDto<bool>> RemoveStudentFromSectionAsync(int sectionId, string studentId);

    /// <summary>
    /// Deletes a section and its associated data from the system.
    /// </summary>
    Task<SectionResultDto<bool>> DeleteSectionAsync(int sectionId);
}