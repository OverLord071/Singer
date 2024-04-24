using Microsoft.AspNetCore.Mvc;
using Singer.Domain;
using Singer.Interfaces;

namespace Singer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IMessage _emailService;

    public EmailController(IMessage emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public IActionResult SendEmail(Email email)
    {
        _emailService.SendEmail(email);
        return Ok(email);
    }
}
