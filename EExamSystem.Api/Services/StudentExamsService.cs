using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.StudentDashboard;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Services;

/// <inheritdoc cref="IStudentExamsService"/>
public class StudentExamsService : IStudentExamsService
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public StudentExamsService(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Queries the database for exams assigned to the student's sections that have not yet expired.
    /// </summary>
    public async Task<StudentExamsResultDto<IEnumerable<ActiveExamDto>>> GetActiveExamsAsync(string studentId)
    {
        var user = await _userManager.FindByIdAsync(studentId);
        if (user == null) return new StudentExamsResultDto<IEnumerable<ActiveExamDto>> { IsSuccess = false, Message = "StudentNotFound" };

        // 1. Find exams assigned to sections where the student is enrolled, AND the exam hasn't ended.
        var activeExams = await _context.Exams
            .Where(e => e.AssignedSections.Any(s => s.Students.Any(u => u.Id == studentId)))
            .Where(e => e.EndTime > DateTime.UtcNow) // Only show exams that are still open or upcoming
            .Include(e => e.Course)
            .Select(e => new ActiveExamDto
            {
                ExamId = e.Id,
                Title = e.Title,
                CourseName = e.Course!.Name,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                DurationMinutes = e.DurationMinutes,
                MaxScore = e.MaxScore
            })
            .ToListAsync();

        return new StudentExamsResultDto<IEnumerable<ActiveExamDto>> { IsSuccess = true, Data = activeExams };
    }

    /// <summary>
    /// Queries the StudentExamSessions table for completed attempts.
    /// </summary>
    public async Task<StudentExamsResultDto<IEnumerable<ExamHistoryDto>>> GetExamHistoryAsync(string studentId)
    {
        var user = await _userManager.FindByIdAsync(studentId);
        if (user == null) return new StudentExamsResultDto<IEnumerable<ExamHistoryDto>> { IsSuccess = false, Message = "StudentNotFound" };

        // 1. Fetch completed sessions for this specific student
        var history = await _context.ExamSessions
            .Include(ses => ses.Exam)
            .ThenInclude(e => e!.Course)
            .Where(ses => ses.StudentId == studentId && ses.CompletedAt != null)
            .Select(ses => new ExamHistoryDto
            {
                SessionId = ses.Id,
                ExamId = ses.ExamId,
                Title = ses.Exam!.Title,
                CourseName = ses.Exam.Course!.Name,
                StartedAt = ses.StartedAt,
                CompletedAt = ses.CompletedAt,
                FinalScore = ses.FinalScore,
                MaxScore = ses.Exam.MaxScore
            })
            .ToListAsync();

        return new StudentExamsResultDto<IEnumerable<ExamHistoryDto>> { IsSuccess = true, Data = history };
    }

    /// <summary>
    /// Retrieves pre-test lobby details, ensuring the student actually has authorization to view this exam.
    /// </summary>
    public async Task<StudentExamsResultDto<ExamPreTestDetailsDto>> GetExamDetailsAsync(string studentId, int examId)
    {
        var user = await _userManager.FindByIdAsync(studentId);
        if (user == null) return new StudentExamsResultDto<ExamPreTestDetailsDto> { IsSuccess = false, Message = "StudentNotFound" };

        // 1. Fetch the exam, validating that the student's section is assigned to it
        var exam = await _context.Exams
            .Include(e => e.Course)
            .Include(e => e.CoveredChapters)
                .ThenInclude(c => c.Questions)
            .Where(e => e.Id == examId && e.AssignedSections.Any(s => s.Students.Any(u => u.Id == studentId)))
            .FirstOrDefaultAsync();

        if (exam == null)
            return new StudentExamsResultDto<ExamPreTestDetailsDto> { IsSuccess = false, Message = "ExamNotFoundOrUnauthorized" };

        // 2. Calculate total questions dynamically from the covered chapters
        var totalQuestions = exam.CoveredChapters.SelectMany(c => c.Questions).Count();

        var details = new ExamPreTestDetailsDto
        {
            ExamId = exam.Id,
            Title = exam.Title,
            CourseName = exam.Course!.Name,
            StartTime = exam.StartTime,
            EndTime = exam.EndTime,
            DurationMinutes = exam.DurationMinutes,
            MaxScore = exam.MaxScore,
            PassingScore = exam.PassingScore,
            TotalQuestions = totalQuestions
        };

        return new StudentExamsResultDto<ExamPreTestDetailsDto> { IsSuccess = true, Data = details };
    }
}