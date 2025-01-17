using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Singer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PdfController : ControllerBase
{
    private readonly string filePath = @"C:/Users/chris/Downloads/facturabateria.pdf";

    [HttpGet("donwload")]
    public IActionResult DonwloadPdf()
    {
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "El archivo no existe." });
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var fileName = "ImpresoraBraile.pdf";

        return File(fileBytes, "application/pdf", fileName);
    }
}
