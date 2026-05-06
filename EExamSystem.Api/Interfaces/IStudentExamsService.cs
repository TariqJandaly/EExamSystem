using EExamSystem.Shared.DTOs.StudentDashboard;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Defines the contract for the student-facing dashboard, retrieving active exams and past results.
/// </summary>
public interface IStudentExamsService
{
    /// <summary>
    /// Retrieves all upcoming and active exams assigned to the student's enrolled sections.
    /// </summary>
    Task<StudentExamsResultDto<IEnumerable<ActiveExamDto>>> GetActiveExamsAsync(string studentId);

    /// <summary>
    /// Retrieves the student's completed exam sessions and grades.
    /// </summary>
    Task<StudentExamsResultDto<IEnumerable<ExamHistoryDto>>> GetExamHistoryAsync(string studentId);

    /// <summary>
    /// Retrieves the detailed instructions and metadata for a specific exam before starting.
    /// </summary>
    Task<StudentExamsResultDto<ExamPreTestDetailsDto>> GetExamDetailsAsync(string studentId, int examId);
}