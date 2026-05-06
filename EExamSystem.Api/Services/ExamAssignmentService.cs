using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Assignments;
using EExamSystem.Shared.DTOs.Exams;
using EExamSystem.Shared.DTOs.Sections;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EExamSystem.Api.Services;

/// <inheritdoc cref="IExamAssignmentService"/>
public class ExamAssignmentService : IExamAssignmentService
{
    private readonly AppDbContext _context;

    public ExamAssignmentService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Validates existence, ensures course consistency, and assigns the exam to the section.
    /// </summary>
    public async Task<AssignmentResultDto<bool>> AssignExamToSectionAsync(int examId, int sectionId)
    {
        // Locate the exam and include the current section assignments
        var exam = await _context.Exams
            .Include(e => e.AssignedSections)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null)
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "ExamNotFound" };

        // Locate the section
        var section = await _context.Sections.FindAsync(sectionId);
        
        if (section == null)
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "SectionNotFound" };

        // BUSINESS RULE: Cross-Course Assignment Prevention
        if (exam.CourseId != section.CourseId)
        {
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "CourseMismatch" };
        }

        // Initial check for existing assignment
        if (exam.AssignedSections.Any(s => s.Id == sectionId))
        {
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "ExamAlreadyAssigned" };
        }

        // Add section to the tracking collection
        exam.AssignedSections.Add(section);

        try
        {
            // Attempt to persist changes to the database
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when
            (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "ExamAlreadyAssigned" };
        }

        return new AssignmentResultDto<bool> { IsSuccess = true, Data = true, Message = "AssignmentSuccess" };
    }

    /// <summary>
    /// Unlinks an exam from a section.
    /// </summary>
    public async Task<AssignmentResultDto<bool>> RevokeExamFromSectionAsync(int examId, int sectionId)
    {
        // Find exam and its assigned sections
        var exam = await _context.Exams
            .Include(e => e.AssignedSections)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null)
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "ExamNotFound" };

        // Find the specific section link
        var section = exam.AssignedSections.FirstOrDefault(s => s.Id == sectionId);
        if (section == null)
        {
            return new AssignmentResultDto<bool> { IsSuccess = false, Message = "AssignmentNotFound" };
        }

        // Remove and save
        exam.AssignedSections.Remove(section);
        await _context.SaveChangesAsync();

        return new AssignmentResultDto<bool> { IsSuccess = true, Data = true, Message = "RevokeSuccess" };
    }

    /// <summary>
    /// Fetches all sections assigned to a specific exam.
    /// </summary>
    public async Task<AssignmentResultDto<IEnumerable<SectionDto>>> GetSectionsForExamAsync(int examId)
    {
        var examExists = await _context.Exams.AnyAsync(e => e.Id == examId);
        if (!examExists)
            return new AssignmentResultDto<IEnumerable<SectionDto>> { IsSuccess = false, Message = "ExamNotFound" };

        var sections = await _context.Sections
            .Where(s => s.AssignedExams.Any(e => e.Id == examId))
            .Select(s => new SectionDto
            {
                Id = s.Id,
                Name = s.Name,
                CourseId = s.CourseId
            })
            .ToListAsync();

        return new AssignmentResultDto<IEnumerable<SectionDto>> { IsSuccess = true, Data = sections };
    }

    /// <summary>
    /// Fetches all exams assigned to a specific section.
    /// </summary>
    public async Task<AssignmentResultDto<IEnumerable<ExamDto>>> GetExamsForSectionAsync(int sectionId)
    {
        var sectionExists = await _context.Sections.AnyAsync(s => s.Id == sectionId);
        if (!sectionExists)
            return new AssignmentResultDto<IEnumerable<ExamDto>> { IsSuccess = false, Message = "SectionNotFound" };

        var exams = await _context.Exams
            .Where(e => e.AssignedSections.Any(s => s.Id == sectionId))
            .Select(e => new ExamDto
            {
                Id = e.Id,
                Title = e.Title,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                DurationMinutes = e.DurationMinutes,
                MaxScore = e.MaxScore,
                PassingScore = e.PassingScore,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return new AssignmentResultDto<IEnumerable<ExamDto>> { IsSuccess = true, Data = exams };
    }
}