using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singer.Interfaces;
using Singer.Utilities.Utils;

namespace Singer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SignerController : ControllerBase
{
    private readonly ISignerApplication signerApplication;

    public SignerController(ISignerApplication signerApplication) 
    {
        this.signerApplication = signerApplication;
    }

    [HttpPost]
    [Route("verify-certificate")]
    [ServiceFilter(typeof(JwtAuthorizationFilter))]
    public async Task<IActionResult> VerifyCertificate(IFormFile certificateFile, string password) 
    {
        if (certificateFile == null || certificateFile.Length == 0 || string.IsNullOrEmpty(password)) 
        {
            return BadRequest("Se requieren tanto el archivo del certificado como la contraseña");
        }
        try
        {
            var certificate = await signerApplication.ValidateCertificate(certificateFile, password);
            return Ok(certificate);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al procesar el certificado: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("sign-pdf")]
    public async Task<IActionResult> SingPdf(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int positionX, int positionY)
    {
        if (certificateFile == null || certificateFile.Length == 0 || pdfFile == null || pdfFile.Length == 0 || string.IsNullOrEmpty(password))
        {
            return BadRequest("Se requiren tanto el archivo de certicado, contraseña, y el archivo pdf");
        }
        try
        {
            var signedPdf = await signerApplication.SignPdfDocumentAsync2(certificateFile, password, pdfFile, reason, location, positionX, positionY);
            return File(signedPdf, "application/pdf", "signed-document.pdf");
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Argumento Invalido: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al firmar el documento PDF: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("verify-pdf")]
    public async Task<IActionResult> VerifyPdf(IFormFile pdfFile)
    {
        if (pdfFile == null || pdfFile.Length == 0)
        {
            return BadRequest("Se requiere un archivo PDF.");
        }
        try
        {
            var document = await signerApplication.VerifyPdfSignature(pdfFile);
            return Ok(document);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Arguemnto invalido: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al vetificar el documento PDF: {ex.Message}");
        }
    }
}
