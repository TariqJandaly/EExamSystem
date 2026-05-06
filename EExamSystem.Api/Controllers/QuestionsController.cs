using EExamSystem.Api.Data;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Questions;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages the creation, retrieval, updating, and deletion of testbank questions and their multiple-choice options.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1")]
public class QuestionsController(AppDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves all questions and their options for a specific testbank chapter.
    /// </summary>
    /// <param name="testbankChapterId">The ID of the chapter containing the questions.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="QuestionDto"/>.</returns>
    /// <response code="200">Successfully retrieved the question list.</response>
    /// <response code="404">
    /// The chapter was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ChapterNotFound</c> – No chapter exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("chapters/{testbankChapterId}/questions")]
    [ProducesResponseType(typeof(ServiceResponse<IEnumerable<QuestionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<IEnumerable<QuestionDto>>>> GetQuestionsForChapter(int testbankChapterId)
    {
        var chapterExists = await context.TestbankChapters.AnyAsync(c => c.Id == testbankChapterId);
        if (!chapterExists)
            return NotFound(new ErrorServiceResponse { Message = "ChapterNotFound", StatusCode = 404 });

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

        return Ok(new ServiceResponse<IEnumerable<QuestionDto>> { Data = questions });
    }

    /// <summary>
    /// Creates a new question with multiple-choice options inside a specific chapter.
    /// </summary>
    /// <param name="testbankChapterId">The ID of the chapter where the question will be created.</param>
    /// <param name="newQuestion">The question and options payload. See <see cref="QuestionCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the newly created <see cref="QuestionDto"/> with generated IDs.</returns>
    /// <response code="201">
    /// Question created successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>QuestionCreatedSuccess</c> – The question and its options were persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">The request payload failed model validation.</response>
    /// <response code="404">
    /// The parent chapter was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>ChapterNotFound</c> – No chapter exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("chapters/{testbankChapterId}/questions")]
    [ProducesResponseType(typeof(ServiceResponse<QuestionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<QuestionDto>>> CreateQuestion(int testbankChapterId, [FromBody] QuestionCreateDto newQuestion)
    {
        var chapterExists = await context.TestbankChapters.AnyAsync(c => c.Id == testbankChapterId);
        if (!chapterExists)
            return NotFound(new ErrorServiceResponse { Message = "ChapterNotFound", StatusCode = 404 });

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

        return CreatedAtAction(nameof(GetQuestion), new { id = question.Id },
            new ServiceResponse<QuestionDto> { Data = createdDto, Message = "QuestionCreatedSuccess", StatusCode = 201 });
    }

    /// <summary>
    /// Retrieves a specific question and all its options by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the question.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the matching <see cref="QuestionDto"/>.</returns>
    /// <response code="200">Successfully retrieved the question.</response>
    /// <response code="404">
    /// The question was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>QuestionNotFound</c> – No question exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("questions/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<QuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<QuestionDto>>> GetQuestion(int id)
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

        if (question == null)
            return NotFound(new ErrorServiceResponse { Message = "QuestionNotFound", StatusCode = 404 });

        return Ok(new ServiceResponse<QuestionDto> { Data = question });
    }

    /// <summary>
    /// Updates an existing question's content, points, and options.
    /// </summary>
    /// <remarks>
    /// ⚠️ <b>Warning:</b> This is a full replacement. All existing options are deleted and recreated from the provided payload.
    /// </remarks>
    /// <param name="id">The ID of the question to update.</param>
    /// <param name="updatedQuestion">The updated question and options payload. See <see cref="QuestionCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Question updated successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>QuestionUpdatedSuccess</c> – The question and its options were replaced and persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">The request payload failed model validation.</response>
    /// <response code="404">
    /// The question was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>QuestionNotFound</c> – No question exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpPut("questions/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuestion(int id, [FromBody] QuestionCreateDto updatedQuestion)
    {
        var question = await context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound(new ErrorServiceResponse { Message = "QuestionNotFound", StatusCode = 404 });

        question.Content = updatedQuestion.Content;
        question.Points = updatedQuestion.Points;

        context.QuestionOptions.RemoveRange(question.Options);
        question.Options = updatedQuestion.Options.Select(o => new QuestionOption
        {
            Text = o.Text,
            IsCorrect = o.IsCorrect
        }).ToList();

        await context.SaveChangesAsync();
        return Ok(new ServiceResponse<bool> { Data = true, Message = "QuestionUpdatedSuccess" });
    }

    /// <summary>
    /// Permanently deletes a question and all its associated options.
    /// </summary>
    /// <remarks>
    /// ⚠️ <b>Warning:</b> This is a cascading delete. All options belonging to this question will be permanently removed.
    /// </remarks>
    /// <param name="id">The ID of the question to delete.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> with <c>Data: true</c> on success.</returns>
    /// <response code="200">
    /// Question deleted successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>QuestionDeletedSuccess</c> – The question and all its options were removed.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The question was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>QuestionNotFound</c> – No question exists with the given ID.</item>
    /// </list>
    /// </response>
    [Authorize(Roles = "Instructor,Admin")]
    [HttpDelete("questions/{id}")]
    [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteQuestion(int id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question == null)
            return NotFound(new ErrorServiceResponse { Message = "QuestionNotFound", StatusCode = 404 });

        context.Questions.Remove(question);
        await context.SaveChangesAsync();

        return Ok(new ServiceResponse<bool> { Data = true, Message = "QuestionDeletedSuccess" });
    }
}