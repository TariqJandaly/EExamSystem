

using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Testbanks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EExamSystem.Shared.DTOs;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Controller for managing testbanks, which are collections of exam questions associated with specific courses.
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
    /// Retrieves a list of all testbanks. Accessible to users with Admin or Instructor roles.
    /// </summary>
    /// <returns>A service response containing the list of testbanks or an error message.</returns>
    /// <response code="200">Returns the list of testbanks if the request is successful.</response>
    /// <response code="400">Returns an error message if the request fails.</response>
    [HttpGet]
    [Authorize(Roles = "Chair,Admin,Instructor")]
    public async Task<IActionResult> GetAllTestbanks()
    {
        var result = await _testbankService.GetAllTestbanksAsync();
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Retrieves a testbank by its ID. Accessible to users with Admin or Instructor roles.
    /// </summary>
    /// <param name="id">The ID of the testbank to retrieve.</param>
    /// <returns>A service response containing the testbank or an error message.</returns>
    /// <response code="200">Returns the testbank if the request is successful.</response>
    /// <response code="404">Returns an error message if the testbank is not found.</response>
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
    /// Creates a new testbank with the provided details. Accessible only to users with the Admin role.
    /// </summary>
    /// <param name="testbankCreateDto">The details for the new testbank.</param>
    /// <returns>A service response containing the created testbank or an error message.</returns>
    /// <response code="200">Returns the created testbank if the request is successful.</response>
    /// <response code="400">Returns an error message if the request fails.</response>
    [HttpPost]
    [Authorize(Roles = "Chair,Admin")]
    public async Task<IActionResult> CreateTestbank([FromBody] TestbankCreateDto testbankCreateDto)
    {
        var result = await _testbankService.CreateTestbankAsync(testbankCreateDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Deletes a testbank by its ID. Accessible only to users with the Chair or Admin roles.
    /// </summary>
    /// <param name="id">The ID of the testbank to delete.</param>
    /// <returns>A service response indicating the result of the operation.</returns>
    /// <response code="200">Returns a success message if the request is successful.</response>
    /// <response code="404">Returns an error message if the testbank is not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Chair,Admin")]
    public async Task<IActionResult> DeleteTestbank(int id)
    {
        var result = await _testbankService.DeleteTestbankAsync(id);
        return StatusCode(result.StatusCode, result);
    }


    /// <summary>
    /// Updates an existing testbank with the provided details. Accessible only to users with the Chair, Admin and Instructor roles.
    /// </summary>
    /// <param name="id">The ID of the testbank to update.</param>
    /// <param name="testbankCreateDto">The updated details for the testbank.</param>
    /// <returns>A service response containing the updated testbank or an error message.</returns>
    /// <response code="200">Returns the updated testbank if the request is successful.</response>
    /// <response code="404">Returns an error message if the testbank is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Chair,Admin,Instructor")]
    public async Task<IActionResult> UpdateTestbank(int id, [FromBody] TestbankCreateDto testbankCreateDto)
    {
        var result = await _testbankService.UpdateTestbankAsync(id, testbankCreateDto);

        return StatusCode(result.StatusCode, result);
    }
}