using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Sections;
using EExamSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Services;

/// <inheritdoc cref="ISectionService"/>
public class SectionService : ISectionService
{
    private readonly AppDbContext _context;

    public SectionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SectionResultDto<IEnumerable<SectionDto>>> GetSectionsByCourseAsync(int courseId)
    {
        // 1. Query the database for sections matching the course ID
        var sections = await _context.Sections
            .Where(s => s.CourseId == courseId)
            .Select(s => new SectionDto
            {
                Id = s.Id,
                Name = s.Name,
                CourseId = s.CourseId
            })
            .ToListAsync();

        // 2. Return the raw collection data
        return new SectionResultDto<IEnumerable<SectionDto>>
        {
            IsSuccess = true,
            Data = sections
        };
    }

    public async Task<SectionResultDto<SectionDto>> GetSectionByIdAsync(int id)
    {
        // 1. Attempt to find the specific section entity
        var section = await _context.Sections.FindAsync(id);

        if (section == null)
        {
            return new SectionResultDto<SectionDto> 
            { 
                IsSuccess = false, 
                Message = "SectionNotFound" 
            };
        }

        // 2. Map entity to DTO and return
        return new SectionResultDto<SectionDto>
        {
            IsSuccess = true,
            Data = new SectionDto 
            { 
                Id = section.Id, 
                Name = section.Name, 
                CourseId = section.CourseId 
            }
        };
    }

    public async Task<SectionResultDto<SectionDto>> CreateSectionAsync(CreateSectionDto model)
    {
        // 1. Verify that the parent course exists before creation
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == model.CourseId);
        if (!courseExists)
        {
            return new SectionResultDto<SectionDto> 
            { 
                IsSuccess = false, 
                Message = "CourseNotFound" 
            };
        }

        // 2. Initialize and save the new section entity
        var section = new Section
        {
            Name = model.Name,
            CourseId = model.CourseId
        };

        _context.Sections.Add(section);
        await _context.SaveChangesAsync();

        // 3. Return the newly created record
        return new SectionResultDto<SectionDto>
        {
            IsSuccess = true,
            Data = new SectionDto 
            { 
                Id = section.Id, 
                Name = section.Name, 
                CourseId = section.CourseId 
            },
            Message = "SectionCreatedSuccess"
        };
    }

    public async Task<SectionResultDto<bool>> AddStudentToSectionAsync(int sectionId, string studentId)
    {
        // 1. Load section and existing students to check for duplicates
        var section = await _context.Sections
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section == null) return new SectionResultDto<bool> { IsSuccess = false, Message = "SectionNotFound" };

        // 2. Verify the student exists in the identity system
        var student = await _context.Users.FindAsync(studentId);
        if (student == null) return new SectionResultDto<bool> { IsSuccess = false, Message = "StudentNotFound" };

        // 3. Prevent redundant enrollment entries
        if (section.Students.Any(u => u.Id == studentId))
        {
            return new SectionResultDto<bool> { IsSuccess = false, Message = "StudentAlreadyEnrolled" };
        }

        // 4. Update relationship and persist changes
        section.Students.Add(student);
        await _context.SaveChangesAsync();

        return new SectionResultDto<bool> { IsSuccess = true, Data = true, Message = "EnrollmentSuccess" };
    }

    public async Task<SectionResultDto<bool>> RemoveStudentFromSectionAsync(int sectionId, string studentId)
    {
        // 1. Load section students to verify membership
        var section = await _context.Sections
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section == null) return new SectionResultDto<bool> { IsSuccess = false, Message = "SectionNotFound" };

        // 2. Find the student link within the specific section collection
        var student = section.Students.FirstOrDefault(u => u.Id == studentId);
        if (student == null) return new SectionResultDto<bool> { IsSuccess = false, Message = "EnrollmentNotFound" };

        // 3. Remove the link and save
        section.Students.Remove(student);
        await _context.SaveChangesAsync();

        return new SectionResultDto<bool> { IsSuccess = true, Data = true, Message = "RemovalSuccess" };
    }

    public async Task<SectionResultDto<bool>> DeleteSectionAsync(int sectionId)
    {
        // 1. Locate the section for deletion
        var section = await _context.Sections.FindAsync(sectionId);
        if (section == null)
        {
            return new SectionResultDto<bool> { IsSuccess = false, Message = "SectionNotFound" };
        }

        // 2. Perform the delete operation
        _context.Sections.Remove(section);
        await _context.SaveChangesAsync();

        return new SectionResultDto<bool> { IsSuccess = true, Data = true, Message = "DeleteSuccess" };
    }
}