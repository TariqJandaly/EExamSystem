using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Sessions;
using EExamSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Services;

/// <inheritdoc cref="IExamSessionService"/>
public class ExamSessionService : IExamSessionService
{
    private readonly AppDbContext _context;

    public ExamSessionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SessionResultDto<ActiveSessionDto>> StartOrResumeSessionAsync(string studentId, int examId)
    {
        // Fetch exam with its questions and options
        var exam = await _context.Exams
            .Include(e => e.CoveredChapters)
                .ThenInclude(c => c.Questions)
                    .ThenInclude(q => q.Options)
            .Where(e => e.Id == examId && e.AssignedSections.Any(s => s.Students.Any(u => u.Id == studentId)))
            .FirstOrDefaultAsync();

        if (exam == null || exam.StartTime > DateTime.UtcNow || exam.EndTime < DateTime.UtcNow)
        {
            return new SessionResultDto<ActiveSessionDto> { IsSuccess = false, Message = "ExamUnavailableOrUnauthorized" };
        }

        // Check if a session already exists for this student/exam combo
        var session = await _context.ExamSessions
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.ExamId == examId);

        // Create session if it's their first time clicking Start
        if (session == null)
        {
            session = new StudentExamSession
            {
                StudentId = studentId,
                ExamId = examId,
                StartedAt = DateTime.UtcNow
            };
            _context.ExamSessions.Add(session);
            await _context.SaveChangesAsync();
        }
        else if (session.CompletedAt != null)
        {
            return new SessionResultDto<ActiveSessionDto> { IsSuccess = false, Message = "ExamAlreadySubmitted" };
        }

        // Map questions to the safe "Play" DTO (stripping out IsCorrect)
        var safeQuestions = exam.CoveredChapters.SelectMany(c => c.Questions).Select(q => new ExamPlayQuestionDto
        {
            Id = q.Id,
            Content = q.Content,
            Points = q.Points,
            Options = q.Options.Select(o => new ExamPlayOptionDto
            {
                Id = o.Id,
                Text = o.Text
            }).ToList()
        }).ToList();

        // Calculate when the timer runs out
        var deadline = session.StartedAt.AddMinutes(exam.DurationMinutes);

        return new SessionResultDto<ActiveSessionDto>
        {
            IsSuccess = true,
            Data = new ActiveSessionDto
            {
                SessionId = session.Id,
                ExamId = exam.Id,
                Title = exam.Title,
                StartedAt = session.StartedAt,
                SessionDeadline = deadline,
                Questions = safeQuestions
            }
        };
    }

    public async Task<SessionResultDto<bool>> SaveAnswerAsync(string studentId, int sessionId, SubmitAnswerDto answerDto)
    {
        // Fetch session
        var session = await _context.ExamSessions
            .Include(s => s.Exam)
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.StudentId == studentId);

        if (session == null) return new SessionResultDto<bool> { IsSuccess = false, Message = "SessionNotFound" };
        if (session.CompletedAt != null) return new SessionResultDto<bool> { IsSuccess = false, Message = "SessionLocked" };

        // Check if time has expired
        var deadline = session.StartedAt.AddMinutes(session.Exam!.DurationMinutes);
        if (DateTime.UtcNow > deadline)
        {
            return new SessionResultDto<bool> { IsSuccess = false, Message = "TimeExpired" };
        }

        // Evaluate the answer
        var option = await _context.QuestionOptions.FindAsync(answerDto.OptionId);
        if (option == null || option.QuestionId != answerDto.QuestionId)
        {
            return new SessionResultDto<bool> { IsSuccess = false, Message = "InvalidOption" };
        }

        // Upsert (Update if exists, Insert if new)
        var existingAnswer = session.Answers.FirstOrDefault(a => a.QuestionId == answerDto.QuestionId);
        if (existingAnswer != null)
        {
            existingAnswer.SelectedOptionId = answerDto.OptionId;
            existingAnswer.IsCorrect = option.IsCorrect; // Stored securely on backend
        }
        else
        {
            session.Answers.Add(new StudentAnswer
            {
                QuestionId = answerDto.QuestionId,
                SelectedOptionId = answerDto.OptionId,
                IsCorrect = option.IsCorrect
            });
        }

        await _context.SaveChangesAsync();
        return new SessionResultDto<bool> { IsSuccess = true, Data = true };
    }

    public async Task<SessionResultDto<decimal>> SubmitAndGradeSessionAsync(string studentId, int sessionId)
    {
        // Fetch session and answers
        var session = await _context.ExamSessions
            .Include(s => s.Exam)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question) // Need this to access the Points value
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.StudentId == studentId);

        if (session == null) return new SessionResultDto<decimal> { IsSuccess = false, Message = "SessionNotFound" };
        if (session.CompletedAt != null) return new SessionResultDto<decimal> { IsSuccess = false, Message = "SessionAlreadySubmitted" };

        // Calculate Final Score (Sum of points for correct answers)
        decimal finalScore = 0;
        foreach (var answer in session.Answers)
        {
            if (answer.IsCorrect && answer.Question != null)
            {
                finalScore += answer.Question.Points;
            }
        }

        // Cap score at MaxScore just in case of configuration errors
        if (finalScore > session.Exam!.MaxScore) finalScore = session.Exam.MaxScore;

        // Lock session and save
        session.FinalScore = finalScore;
        session.CompletedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return new SessionResultDto<decimal> { IsSuccess = true, Data = finalScore, Message = "GradingComplete" };
    }
}