using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Enrollment;
using EExamSystem.Shared.DTOs.Sections;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly AppDbContext _context;

    public EnrollmentService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Validates existence and links a student user to a specific section.
    /// </summary>
    public async Task<EnrollmentResultDto<bool>> EnrollStudentAsync(int sectionId, string studentId)
    {
        // Locate the section and include the student list to check for existing enrollment
        var section = await _context.Sections
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section == null)
        {
            return new EnrollmentResultDto<bool> { IsSuccess = false, Message = "SectionNotFound" };
        }

        // Locate the student user in the identity system
        var student = await _context.Users.FindAsync(studentId);
        if (student == null)
        {
            return new EnrollmentResultDto<bool> { IsSuccess = false, Message = "StudentNotFound" };
        }

        // Verify that the student isn't already a member of this section
        if (section.Students.Any(u => u.Id == studentId))
        {
            return new EnrollmentResultDto<bool> { IsSuccess = false, Message = "StudentAlreadyEnrolled" };
        }

        // Update the relationship and persist the changes
        section.Students.Add(student);
        await _context.SaveChangesAsync();

        return new EnrollmentResultDto<bool>
        {
            IsSuccess = true,
            Data = true,
            Message = "EnrollmentSuccess"
        };
    }

    /// <summary>
    /// Removes the link between a student user and a section.
    /// </summary>
    public async Task<EnrollmentResultDto<bool>> UnenrollStudentAsync(int sectionId, string studentId)
    {
        // Locate the section and its member list
        var section = await _context.Sections
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section == null)
        {
            return new EnrollmentResultDto<bool> { IsSuccess = false, Message = "SectionNotFound" };
        }

        // Find the specific student entry in the section's list
        var student = section.Students.FirstOrDefault(u => u.Id == studentId);
        if (student == null)
        {
            return new EnrollmentResultDto<bool> { IsSuccess = false, Message = "EnrollmentNotFound" };
        }

        // Remove the student and save changes
        section.Students.Remove(student);
        await _context.SaveChangesAsync();

        return new EnrollmentResultDto<bool>
        {
            IsSuccess = true,
            Data = true,
            Message = "UnenrollmentSuccess"
        };
    }

    /// <summary>
    /// Fetches all sections associated with a student ID.
    /// </summary>
    public async Task<EnrollmentResultDto<IEnumerable<SectionDto>>> GetStudentSectionsAsync(string studentId)
    {
        // Verify the student exists
        var studentExists = await _context.Users.AnyAsync(u => u.Id == studentId);
        if (!studentExists)
        {
            return new EnrollmentResultDto<IEnumerable<SectionDto>> { IsSuccess = false, Message = "StudentNotFound" };
        }

        // Query for sections containing this student in their membership list
        var sections = await _context.Sections
            .Where(s => s.Students.Any(u => u.Id == studentId))
            .Select(s => new SectionDto
            {
                Id = s.Id,
                Name = s.Name,
                CourseId = s.CourseId
            })
            .ToListAsync();

        // Return the collection
        return new EnrollmentResultDto<IEnumerable<SectionDto>>
        {
            IsSuccess = true,
            Data = sections
        };
    }
}