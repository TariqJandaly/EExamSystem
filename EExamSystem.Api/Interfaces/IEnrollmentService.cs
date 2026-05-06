using EExamSystem.Shared.DTOs.Enrollment;
using EExamSystem.Shared.DTOs.Sections;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Defines the contract for student enrollment and membership logic within course sections.
/// </summary>
public interface IEnrollmentService
{
    /// <summary>
    /// Adds a student to a section collection.
    /// </summary>
    Task<EnrollmentResultDto<bool>> EnrollStudentAsync(int sectionId, string studentId);

    /// <summary>
    /// Removes a student from a section collection.
    /// </summary>
    Task<EnrollmentResultDto<bool>> UnenrollStudentAsync(int sectionId, string studentId);

    /// <summary>
    /// Retrieves all sections a specific student is enrolled in.
    /// </summary>
    Task<EnrollmentResultDto<IEnumerable<SectionDto>>> GetStudentSectionsAsync(string studentId);
}