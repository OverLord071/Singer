using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Singer.Application;
using Singer.Domain;
using Singer.Interfaces;

namespace Singer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DWController : ControllerBase
{
    private readonly IDWApplication _dwApplication;

    public DWController(IDWApplication dWApplication)
    {
        _dwApplication = dWApplication;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDW userDW)
    {
        try
        {
            var newUser = await _dwApplication.CreateUserDW(userDW);
            return CreatedAtAction(nameof(Register), newUser);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string usernameOrEmail, string password) 
    {
        try 
        {
            var user = await _dwApplication.AuthentificateUser(usernameOrEmail, password);
            return Ok(user);
        }
        catch (Exception ex) 
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("authenticateWithToken")]
    public async Task<IActionResult> AuthenticateWithToken(string token)
    {
        try
        {
            var email = await _dwApplication.AuthentificateWithToken(token);
            return Ok(email);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("sign-pdf")]
    public async Task<IActionResult> SingPdf(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int page, int positionX, int positionY)
    {
        if (certificateFile == null || certificateFile.Length == 0 || pdfFile == null || pdfFile.Length == 0 || string.IsNullOrEmpty(password))
        {
            return BadRequest("Se requiren tanto el archivo de certicado, contraseña, y el archivo pdf");
        }
        try
        {
            var signedPdf = await _dwApplication.SignDocument(certificateFile, password, pdfFile, reason, location, page, positionX, positionY);
            return Ok(signedPdf);
        }
        catch (ArgumentException ex)
        {
            return BadRequest($"Argumento Invalido: {ex.Message}");
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("PKCS12 key store MAC invalid - wrong password or corrupted file"))
            {
                return StatusCode(500, "Contraseña incorrecta o archivo corrupto");
            }
            return StatusCode(500, $"Error al firmar: {ex.Message}");
        }
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword(string email, string validationPin, string newPassword)
    {
        try
        {
            await _dwApplication.ChangePassword(email, validationPin, newPassword);
            return Ok();
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var certificate = await _dwApplication.GetUserById(id);
            return Ok(certificate);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("sendPinValidation")]
    public async Task<IActionResult> SendPinValidation(string email)
    {
        try
        {
            await _dwApplication.SendPinValidation(email);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("createCertificate")]
    public async Task<IActionResult> CreateCertificate(string id, IFormFile certificate, string pinCertificate)
    {
        try
        {
            var result = await _dwApplication.CreateCertificate(id, certificate, pinCertificate);
            return Ok(result);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("verifyEmail")]
    public async Task<IActionResult> VerifyEmail(string email, string verificationCode)
    {
        try
        {
            await _dwApplication.VerifyEmail(email, verificationCode);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("updateCretificate")]
    public async Task<IActionResult> UpdateCertificate(string id, IFormFile certificate, string pinCertificate)
    {
        try
        {
            var result = await _dwApplication.UpdateCertificate(id,certificate, pinCertificate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserDW(string id)
    {
        var result = await _dwApplication.DeleteUser(id);
        if (!result)
        {
            return NotFound("Usuario no encontrado");
        }

        return NoContent();
    }
}
