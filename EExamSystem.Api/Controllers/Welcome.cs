using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("/")]
public class WelcomeController : ControllerBase
{
    [HttpGet]
    public IActionResult GetWelcomeMessage()
    {
        return Ok(new { message = "Hello from the EExam API!" });
    }
}