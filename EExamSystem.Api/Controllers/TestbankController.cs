using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Testbanks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages testbanks — course-level question repositories used as the source pool for exam questions.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TestbankController : ControllerBase
{
    private readonly ITestbankService _testbankService;

    public TestbankController(ITestbankService testbankService)
    {
        _testbankService = testbankService;
    }

    /// <summary>
    /// Retrieves a list of all testbanks in the system. Restricted to Chair, Admin, and Instructor roles.
    /// </summary>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="TestbankDto"/>.</returns>
    /// <response code="200">Successfully retrieved the full testbank list.</response>
    /// <response code="400">
    /// The request could not be processed. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>FetchFailed</c> – An unexpected error occurred while retrieving testbanks.</item>
    /// </list>
    /// </response>
    [HttpGet]
    [Authorize(Roles = "Chair,Admin,Instructor")]
    [ProducesResponseType(typeof(ServiceResponse<List<TestbankDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllTestbanks()
    {
        var result = await _testbankService.GetAllTestbanksAsync();
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Retrieves a specific testbank by its ID. Restricted to Chair, Admin, Instructor, and Student roles.
    /// </summary>
    /// <param name="id">The ID of the testbank to retrieve.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the matching <see cref="TestbankDto"/>.</returns>
    /// <response code="200">Successfully retrieved the testbank.</response>
    /// <response code="404">
    /// The testbank was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankNotFound</c> – No testbank exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Chair,Admin,Instructor,Student")]
    [ProducesResponseType(typeof(ServiceResponse<TestbankDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTestbank(int id)
    {
        var result = await _testbankService.GetTestbankAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Creates a new testbank. Restricted to Chair and Admin roles.
    /// </summary>
    /// <param name="testbankCreateDto">The testbank creation payload. See <see cref="TestbankCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the newly created <see cref="TestbankDto"/>.</returns>
    /// <response code="201">
    /// Testbank created successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankCreatedSuccess</c> – The testbank was persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// The request payload failed validation. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankAlreadyExists</c> – A testbank with the same name or course already exists.</item>
    /// </list>
    /// </response>
    [HttpPost]
    [Authorize(Roles = "Chair,Admin")]
    [ProducesResponseType(typeof(ServiceResponse<TestbankDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTestbank([FromBody] TestbankCreateDto testbankCreateDto)
    {
        var result = await _testbankService.CreateTestbankAsync(testbankCreateDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Permanently deletes a testbank by its ID. Restricted to Chair and Admin roles.
    /// </summary>
    /// <remarks>
    /// ⚠️ <b>Warning:</b> Deleting a testbank will cascade and remove all chapters and questions within it.
    /// </remarks>
    /// <param name="id">The ID of the testbank to delete.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the deleted <see cref="TestbankDto"/> on success.</returns>
    /// <response code="200">
    /// Testbank deleted successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankDeletedSuccess</c> – The testbank and all its contents were removed.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The testbank was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankNotFound</c> – No testbank exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Chair,Admin")]
    [ProducesResponseType(typeof(ServiceResponse<TestbankDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTestbank(int id)
    {
        var result = await _testbankService.DeleteTestbankAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Updates an existing testbank's details. Restricted to Chair, Admin, and Instructor roles.
    /// </summary>
    /// <param name="id">The ID of the testbank to update.</param>
    /// <param name="testbankCreateDto">The updated testbank payload. See <see cref="TestbankCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the updated <see cref="TestbankDto"/>.</returns>
    /// <response code="200">
    /// Testbank updated successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankUpdatedSuccess</c> – Changes were persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// The request payload failed validation. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankAlreadyExists</c> – The updated name conflicts with an existing testbank.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The testbank was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>TestbankNotFound</c> – No testbank exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Chair,Admin,Instructor")]
    [ProducesResponseType(typeof(ServiceResponse<TestbankDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTestbank(int id, [FromBody] TestbankCreateDto testbankCreateDto)
    {
        var result = await _testbankService.UpdateTestbankAsync(id, testbankCreateDto);
        return StatusCode(result.StatusCode, result);
    }
}