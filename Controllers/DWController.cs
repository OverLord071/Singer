using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Singer.Application;
using Singer.Domain;
using Singer.Domain.Dtos;
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
    public async Task<IActionResult> Register(CreateUserDto createUserDto)
    {
        try
        {
            var newUser = await _dwApplication.CreateUserDW(createUserDto);
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

    [HttpGet("verifyPin")]
    public async Task<IActionResult> VerifyCode(string email, string code)
    {
        try
        {
            var result = await _dwApplication.VerifyCode(email, code);
            return Ok(result);
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

    [HttpPut("{documentId}/reject")]
    public async Task<IActionResult> RejectDocument(string documentId, [FromBody] RejectDocumentRequest request)
    {
        if (string.IsNullOrEmpty(request.Reason))
        {
            return BadRequest("El motivo del rechazo es requerido.");
        }

        try
        {
            await _dwApplication.RejectDocumentAsync(documentId, request.Reason);
            return NoContent();

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Se produjo un error al procesar la solicitud.");
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

    [HttpPut("changeStatus/{id}")]
    public async Task<IActionResult> ChangeStatus(Guid id)
    {
        var user = await _dwApplication.ChangeStatusUser(id);

        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado"});
        }

        return Ok(new
        {
            id = user.Id,
            isActive = user.IsActive,
            message = "Estado del usuario actualizado exitosamente"
        });
    }

    [HttpGet("getAllUsers")]
    public async Task<IActionResult> GetAllUsers() 
    {
        var users = await _dwApplication.GetAllUsers();
        return Ok(users);
    }

    [HttpPost("sendPasswordLink")]
    public async Task<IActionResult> SendPasswordLink([FromBody] PasswordRecoveryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("El email es requerido.");
        }

        try
        {
            await _dwApplication.SendLinkRecoveryPassword(request.Email);
            return Ok("Se ha enviado el enlace de recuperación.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al enviar el enlace de recuperación: {ex.Message}");
        }
    }

    [HttpGet("validateToken")]
    public async Task<IActionResult> ValidateToken([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("El token es obligatorio");
        }

        try
        {
            await _dwApplication.ValidateToken(token);
            return Ok(new { message = "El token es válido." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("changePasswordWithToken")]
    public async Task<IActionResult> ChangePasswordWithToken([FromBody] ChangePasswwordRequest request)
    {
        if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("El token y la nueva contraseña son obligatorias.");
        }

        try
        {
            await _dwApplication.ChangePasswordWithToken(request.Token, request.Password);
            return Ok(new { message = "Contraseña cambiada exitosamente." });
        }
        catch (InvalidOperationException ex) 
        {
            return BadRequest( new { error = ex.Message });
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
