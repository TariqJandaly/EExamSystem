using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Results;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Services;

/// <inheritdoc cref="IResultsService"/>
public class ResultsService : IResultsService
{
    private readonly AppDbContext _context;

    public ResultsService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Aggregates student enrollment data and completed exam sessions into a single grading report.
    /// </summary>
    public async Task<GradingResultDto<SectionResultsDto>> GetSectionResultsAsync(int examId, int sectionId)
    {
        // Fetch the exam and ensure it is actually assigned to this section
        var exam = await _context.Exams
            .Include(e => e.AssignedSections)
                .ThenInclude(s => s.Students)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null)
            return new GradingResultDto<SectionResultsDto> { IsSuccess = false, Message = "ExamNotFound" };

        var section = exam.AssignedSections.FirstOrDefault(s => s.Id == sectionId);
        if (section == null)
            return new GradingResultDto<SectionResultsDto> { IsSuccess = false, Message = "SectionNotAssignedToExam" };

        // Fetch all exam sessions for students in this section for this specific exam
        var studentIds = section.Students.Select(s => s.Id).ToList();
        
        var sessions = await _context.ExamSessions
            .Where(s => s.ExamId == examId && studentIds.Contains(s.StudentId))
            .ToListAsync();

        // Map every enrolled student to the report (even if they have no session)
        var studentScores = section.Students.Select(student => 
        {
            var studentSession = sessions.FirstOrDefault(s => s.StudentId == student.Id);
            
            return new StudentScoreDto
            {
                StudentId = student.Id,
                FullName = student.FullName,
                FinalScore = studentSession?.FinalScore,
                CompletedAt = studentSession?.CompletedAt
            };
        }).OrderBy(s => s.FullName).ToList(); // Sort alphabetically by default

        // Build the final report
        var report = new SectionResultsDto
        {
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            MaxScore = exam.MaxScore,
            PassingScore = exam.PassingScore,
            SectionName = section.Name,
            StudentScores = studentScores
        };

        return new GradingResultDto<SectionResultsDto>
        {
            IsSuccess = true,
            Data = report
        };
    }
}