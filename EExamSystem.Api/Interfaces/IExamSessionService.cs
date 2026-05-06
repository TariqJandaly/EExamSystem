using EExamSystem.Shared.DTOs.Sessions;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Defines the contract for the core test engine: starting exams, saving answers, and grading.
/// </summary>
public interface IExamSessionService
{
    /// <summary>
    /// Starts a new session or resumes an existing unsubmitted session.
    /// </summary>
    Task<SessionResultDto<ActiveSessionDto>> StartOrResumeSessionAsync(string studentId, int examId);

    /// <summary>
    /// Auto-saves a single answer choice. Upserts if the student changes their mind.
    /// </summary>
    Task<SessionResultDto<bool>> SaveAnswerAsync(string studentId, int sessionId, SubmitAnswerDto answerDto);

    /// <summary>
    /// Locks the session and calculates the final grade.
    /// </summary>
    Task<SessionResultDto<decimal>> SubmitAndGradeSessionAsync(string studentId, int sessionId);
}