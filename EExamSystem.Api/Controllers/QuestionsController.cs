using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs.Questions;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the creation, retrieval, updating, and deletion of exam questions and their associated options.
/// </summary>
[ApiController]
[Route("api/v1")]
public class QuestionsController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all questions and their associated options for a specific testbank chapter.
    /// </summary>
    /// <param name="testbankChapterId">The ID of the chapter containing the questions.</param>
    /// <returns>A list of QuestionDto objects.</returns>
    /// <response code="200">Returns the list of questions successfully.</response>
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
    
    /// <summary>
    /// Creates a new question along with its multiple-choice options in a specific chapter.
    /// </summary>
    /// <param name="testbankChapterId">The ID of the chapter where the question will be created.</param>
    /// <param name="newQuestion">The question and options payload.</param>
    /// <returns>The newly created QuestionDto.</returns>
    /// <response code="201">Returns the created question with generated IDs.</response>
    /// <response code="400">If the payload fails validation.</response>
    /// <response code="404">If the specified chapter does not exist.</response>
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
        
        // This generates the IDs for both the Question and all its Options
        await context.SaveChangesAsync(); 

        // Map it back to a DTO to return to the frontend
        var createdDto = new QuestionDto
        {
            Id = question.Id,
            Content = question.Content,
            Points = question.Points,
            Options = question.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, createdDto);
    }
    
    /// <summary>
    /// Retrieves a specific question and its options by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the question.</param>
    /// <returns>A single QuestionDto.</returns>
    /// <response code="200">Returns the requested question.</response>
    /// <response code="404">If the question does not exist.</response>
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

        if (question == null) return NotFound($"Question with ID {id} was not found.");
        return Ok(question);
    }
    
    /// <summary>
    /// Updates an existing question. Replaces all existing options with the newly provided ones.
    /// </summary>
    /// <param name="id">The ID of the question to update.</param>
    /// <param name="updatedQuestion">The updated question and options payload.</param>
    /// <response code="204">Successfully updated the question.</response>
    /// <response code="400">If the payload fails validation.</response>
    /// <response code="404">If the question does not exist.</response>
    [HttpPut("questions/{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionCreateDto updatedQuestion)
    {
        var question = await context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null) return NotFound($"Question with ID {id} was not found.");

        // Update parent properties
        question.Content = updatedQuestion.Content;
        question.Points = updatedQuestion.Points;

        // Wipe old options and insert new ones
        context.QuestionOptions.RemoveRange(question.Options);
        
        question.Options = updatedQuestion.Options.Select(o => new QuestionOption
        {
            Text = o.Text,
            IsCorrect = o.IsCorrect
        }).ToList();

        await context.SaveChangesAsync();
        return NoContent();
    }
    
    /// <summary>
    /// Deletes a specific question and cascades the deletion to all its options.
    /// </summary>
    /// <param name="id">The ID of the question to delete.</param>
    /// <response code="204">Successfully deleted the question.</response>
    /// <response code="404">If the question does not exist.</response>
    [HttpDelete("questions/{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question == null) return NotFound($"Question with ID {id} was not found.");

        context.Questions.Remove(question);
        await context.SaveChangesAsync();

        return NoContent();
    }
}