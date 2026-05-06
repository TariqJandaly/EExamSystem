using Microsoft.EntityFrameworkCore;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EExamSystem.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Testbank> Testbanks { get; set; }
    public DbSet<TestbankChapter> TestbankChapters { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<StudentExamSession> ExamSessions { get; set; }
    public DbSet<StudentAnswer> StudentAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER", ConcurrencyStamp = "static-1" },
            new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT", ConcurrencyStamp = "static-2" },
            new IdentityRole { Id = "3", Name = "Instructor", NormalizedName = "INSTRUCTOR", ConcurrencyStamp = "static-3" },
            new IdentityRole { Id = "4", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "static-4" }
        );
        
        // Prevent duplicate sessions
        modelBuilder.Entity<StudentExamSession>().HasIndex(s => new { s.StudentId, s.ExamId }).IsUnique();
        // Prevent duplicate answers
        modelBuilder.Entity<StudentAnswer>().HasIndex(a => new { a.SessionId, a.QuestionId }).IsUnique();
    }
}