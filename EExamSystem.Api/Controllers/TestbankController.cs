

using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Testbanks;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestbankController : ControllerBase
{

    private readonly ITestbankService _testbankService;

    public TestbankController(ITestbankService testbankService)
    {
        _testbankService = testbankService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTestbanks()
    {
        var result = await _testbankService.GetAllTestbanksAsync();
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTestbank(int id)
    {
        var result = await _testbankService.GetTestbankAsync(id);

        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestbank([FromBody] TestbankCreateDto testbankCreateDto)
    {
        var result = await _testbankService.CreateTestbankAsync(testbankCreateDto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTestbank(int id)
    {
        var result = await _testbankService.DeleteTestbankAsync(id);
        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTestbank(int id, [FromBody] TestbankCreateDto testbankCreateDto)
    {
        var result = await _testbankService.UpdateTestbankAsync(id, testbankCreateDto);
        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }
}