using EExamSystem.Shared.DTOs.Results;

namespace EExamSystem.Api.Interfaces;

/// <summary>
/// Defines the contract for generating grading reports and retrieving student exam results.
/// </summary>
public interface IResultsService
{
    /// <summary>
    /// Retrieves a comprehensive grading report for all students in a specific section for a given exam.
    /// </summary>
    Task<GradingResultDto<SectionResultsDto>> GetSectionResultsAsync(int examId, int sectionId);
}