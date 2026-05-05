using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs.Questions;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;
/// <summary>
/// Manages the creation, retrieval, updating, and deletion of university courses.
/// </summary>
[ApiController]
[Route("api/v1")]
public class QuestionsController(AppDbContext context) : ControllerBase
{
    [HttpGet("chapters/{testbankChapterId}/questions")]
    public async Task<IActionResult> GetQuestionsForChapter(int testbankChapterId)
    {
        var questions = await context.Questions
            .Include(q => q.Options)
            .Where(q => q.TestbankChapterId == testbankChapterId)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Content = q.Content,
                Points = q.Points,
                Options = q.Options.Select(o => new OptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            })
            .ToListAsync();

        return Ok(questions);
    }
    
    [HttpPost("chapters/{testbankChapterId}/questions")]
    public async Task<IActionResult> CreateQuestion(int testbankChapterId, [FromBody] QuestionCreateDto newQuestion)
    {
        var chapterExists = await context.TestbankChapters.AnyAsync(c => c.Id == testbankChapterId);
        if (!chapterExists) return NotFound($"Chapter {testbankChapterId} not found.");

        var question = new Question
        {
            TestbankChapterId = testbankChapterId,
            Content = newQuestion.Content,
            Points = newQuestion.Points,
            
            Options = newQuestion.Options.Select(o => new QuestionOption
            {
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        context.Questions.Add(question);
        
        await context.SaveChangesAsync(); 

        return Created($"/api/v1/questions/{question.Id}", new { Message = "Question created." });
    }
    
    [HttpGet("questions/{id}")]
    public async Task<IActionResult> GetQuestion(int id)
    {
        var question = await context.Questions
            .Include(q => q.Options)
            .Where(q => q.Id == id)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Content = q.Content,
                Points = q.Points,
                Options = q.Options.Select(o => new OptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (question == null) return NotFound();
        return Ok(question);
    }
}