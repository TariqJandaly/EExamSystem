using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Execution;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Handles the core business logic for students taking exams, including session management and auto-grading.
/// </summary>
public interface IExamExecutionService
{
    /// <summary>
    /// Initializes a new exam session for a student or resumes an existing active session.
    /// </summary>
    /// <param name="examId">The ID of the exam.</param>
    /// <param name="userId">The integer ID of the enrolled student.</param>
    /// <returns>An ExamSessionDto containing session details and the active timer duration.</returns>
    Task<ServiceResponse<ExamSessionDto>> StartSessionAsync(int examId, int userId);

    /// <summary>
    /// Records or updates a student's answer for a specific question during an active exam session.
    /// </summary>
    /// <param name="sessionId">The ID of the active exam session.</param>
    /// <param name="userId">The integer ID of the student to verify ownership.</param>
    /// <param name="payload">The question and selected option IDs.</param>
    /// <returns>True if the answer was successfully saved.</returns>
    Task<ServiceResponse<bool>> SubmitAnswerAsync(int sessionId, int userId, SubmitAnswerDto payload);

    /// <summary>
    /// Finalizes an active exam session, calculates the total score based on question points, and marks the exam as completed.
    /// </summary>
    /// <param name="sessionId">The ID of the active exam session.</param>
    /// <param name="userId">The integer ID of the student submitting the exam.</param>
    /// <returns>An ExamResultDto containing the final calculated score and pass/fail status.</returns>
    Task<ServiceResponse<ExamResultDto>> GradeAndSubmitExamAsync(int sessionId, int userId);
}