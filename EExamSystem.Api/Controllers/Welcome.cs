using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

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