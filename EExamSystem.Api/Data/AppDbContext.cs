using Microsoft.EntityFrameworkCore;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // modelBuilder.Entity<User>()
        //     .Property(u => u.Role)
        //     .HasConversion<string>();
    }
}