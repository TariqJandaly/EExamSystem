using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EExamSystem.Shared.DTOs;

namespace EExamSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("/")]
public class WelcomeController : ControllerBase
{
    /// <summary>
    /// Retrieves a welcome message from the EExam API.
    /// </summary>
    /// <response code="200">Returns the welcome message successfully.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetWelcomeMessage()
    {
        return Ok(new { message = "Hello from the EExam API!" });
    }
}