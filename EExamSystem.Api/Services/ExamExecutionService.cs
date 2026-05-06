using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Execution;
using EExamSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Services;

/// <summary>
/// Implementation of IExamExecutionService that interacts directly with the AppDbContext.
/// </summary>
public class ExamExecutionService(AppDbContext context) : IExamExecutionService
{
    /// <inheritdoc />
    public async Task<ServiceResponse<ExamSessionDto>> StartSessionAsync(int examId, int userId)
    {
        var exam = await context.Exams
            .Include(e => e.AssignedStudents)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null)
            return new ServiceResponse<ExamSessionDto> { Success = false, Message = "Exam not found.", StatusCode = 404 };

        // Check Enrollment
        if (!exam.AssignedStudents.Any(s => int.Parse(s.Id) == userId))
            return new ServiceResponse<ExamSessionDto> { Success = false, Message = "You are not enrolled in this exam.", StatusCode = 403 };

        // Check Timing
        var now = DateTime.UtcNow;
        if (now < exam.StartTime || now > exam.EndTime)
            return new ServiceResponse<ExamSessionDto> { Success = false, Message = "Exam is not currently active.", StatusCode = 400 };

        // Check if already started
        var existingSession = await context.Set<StudentExamSession>()
            .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == userId);

        if (existingSession != null)
        {
            if (existingSession.CompletedAt != null)
                return new ServiceResponse<ExamSessionDto> { Success = false, Message = "You have already completed this exam.", StatusCode = 400 };
            
            // Resume existing session
            return new ServiceResponse<ExamSessionDto> 
            { 
                Data = new ExamSessionDto { SessionId = existingSession.Id, ExamId = examId, StartedAt = existingSession.StartedAt, DurationMinutes = exam.DurationMinutes },
                Message = "Resumed existing exam session."
            };
        }

        // Create new session
        var session = new StudentExamSession
        {
            ExamId = examId,
            StudentId = userId,
            StartedAt = now
        };

        context.Add(session);
        await context.SaveChangesAsync();

        return new ServiceResponse<ExamSessionDto>
        {
            Data = new ExamSessionDto { SessionId = session.Id, ExamId = examId, StartedAt = session.StartedAt, DurationMinutes = exam.DurationMinutes },
            Message = "Exam started successfully."
        };
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> SubmitAnswerAsync(int sessionId, int userId, SubmitAnswerDto payload)
    {
        var session = await context.Set<StudentExamSession>()
            .Include(s => s.Exam)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.StudentId == userId);

        if (session == null || session.CompletedAt != null)
            return new ServiceResponse<bool> { Success = false, Message = "Active session not found.", StatusCode = 404 };

        // Auto-grade this single answer by looking at the Option table
        var option = await context.QuestionOptions.FindAsync(payload.SelectedOptionId);
        if (option == null || option.QuestionId != payload.QuestionId)
            return new ServiceResponse<bool> { Success = false, Message = "Invalid option or question.", StatusCode = 400 };

        // Upsert Answer (if they change their mind, overwrite the old answer)
        var existingAnswer = await context.Set<StudentAnswer>()
            .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.QuestionId == payload.QuestionId);

        if (existingAnswer != null)
        {
            existingAnswer.SelectedOptionId = payload.SelectedOptionId;
            existingAnswer.IsCorrect = option.IsCorrect;
        }
        else
        {
            context.Add(new StudentAnswer
            {
                SessionId = sessionId,
                QuestionId = payload.QuestionId,
                SelectedOptionId = payload.SelectedOptionId,
                IsCorrect = option.IsCorrect
            });
        }

        await context.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "Answer recorded successfully." };
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<ExamResultDto>> GradeAndSubmitExamAsync(int sessionId, int userId)
    {
        var session = await context.Set<StudentExamSession>()
            .Include(s => s.Exam)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.StudentId == userId);

        if (session == null || session.CompletedAt != null)
            return new ServiceResponse<ExamResultDto> { Success = false, Message = "Active session not found.", StatusCode = 404 };

        // Calculate Grade based on Question Points
        decimal earnedPoints = session.Answers.Where(a => a.IsCorrect && a.Question != null).Sum(a => a.Question!.Points);
        
        session.FinalScore = earnedPoints; 
        session.CompletedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new ServiceResponse<ExamResultDto>
        {
            Data = new ExamResultDto
            {
                SessionId = session.Id,
                FinalScore = (decimal)session.FinalScore,
                MaxScore = session.Exam!.MaxScore,
                Passed = session.FinalScore >= session.Exam.PassingScore,
                CompletedAt = (DateTime)session.CompletedAt
            },
            Message = "Exam submitted successfully."
        };
    }
}