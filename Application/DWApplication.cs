using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Singer.Domain;
using Singer.Infrastructure;
using Singer.Interfaces;
using IMessage = Singer.Interfaces.IMessage;
using Singer.Utilities.Certificate;

namespace Singer.Application;

public class DWApplication : IDWApplication
{
    private readonly SignerDbContext _context;
    private readonly AmazonSecretsManagerClient _secretsManagerClient;
    private readonly IMessage _emailService;
    private readonly CertificateUtils _certificateUtils;

    public DWApplication(SignerDbContext context, AmazonSecretsManagerClient secretsManagerClient, IMessage emailService, CertificateUtils certificateUtils)
    {
        _context = context;
        _secretsManagerClient = secretsManagerClient;
        _emailService = emailService;
        _certificateUtils = certificateUtils;
    }

    public async Task<UserDW> AuthentificateUser(string usernameOrEmail, string password)
    {
        var user = await _context.UsersDw
            .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

        if (user == null)
        {
            throw new Exception("Usuario no registrado.");
        }

        var passwordHasher = new PasswordHasher<UserDW>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new Exception("Contraseña incorrecta.");
        }

        return user;
    }

    public async Task ChangePassword(string email, string validationPin, string newPassword)
    {
        var user = await _context.UsersDw.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) 
        {
            throw new Exception("Usuario no encontrado.");
        }

        if (user.ValidationPin != validationPin) 
        {
            throw new Exception("PIN incorrecto.");
        }

        var passwordHasher = new PasswordHasher<UserDW>();
        user.Password = passwordHasher.HashPassword(user, newPassword);

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<UserDW> CreateUserDW(UserDW userDW)
    {
        var passwordHasher = new PasswordHasher<UserDW>();
        userDW.Password = passwordHasher.HashPassword(userDW, userDW.Password);

        _context.UsersDw.Add(userDW);
        await _context.SaveChangesAsync();

        await SendPinValidation(userDW.Email);

        return userDW;
    }

    public async Task<SignerCertificate> GetUserById(string id)
    {
        var user = await _context.UsersDw.FindAsync(id);

        if (user == null) 
        {
            throw new Exception("Usuario no encontrado.");
        }

        var secretValue = await _secretsManagerClient.GetSecretValueAsync(new GetSecretValueRequest { SecretId = user.CI });
        var parts = secretValue.SecretString.Split(':');
        
        var certificate = new SignerCertificate();
        certificate.Certificate = parts[0];
        certificate.PinCertificate = parts[1];

        return certificate;

    }

    public async Task SendPinValidation(string email)
    {
        var user = await _context.UsersDw.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        var validationPin = new Random().Next(100000, 999999).ToString();

        user.ValidationPin = validationPin;

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var emailRequest = new Email
        {
            Recipient = email,
            Subject = "Codigo de verificacion",
            Body = $"Tu codigo de verificaion es: {validationPin}"
        };

        _emailService.SendEmail(emailRequest);

    }

    public async Task<bool> CreateCertificate(string id, IFormFile certificateFile, string pinCertificate)
    {
        var user = await _context.UsersDw.FindAsync(id);

        if (user == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        if (!user.EmailVerified)
        {
            throw new Exception("El usuario aun no ha sido validado.");
        }

        using var ms = new MemoryStream();
        await certificateFile.CopyToAsync(ms);
        var filesBytes = ms.ToArray();
        string certificate = Convert.ToBase64String(filesBytes);

        var secretRequest = new CreateSecretRequest 
        { 
            Name = user.CI,
            SecretString = certificate + ":" + pinCertificate
        };

        await _secretsManagerClient.CreateSecretAsync(secretRequest);

        return true;
    }

    public async Task VerifyEmail(string email, string verificationCode)
    {
        var user = await _context.UsersDw.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null) 
        {
            throw new Exception("Usuario no encontrado");
        }

        if (user.ValidationPin != verificationCode)
        {
            throw new Exception("Codigo de verificacion incorrecto.");
        }

        user.EmailVerified = true;

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateCertificate(string id, IFormFile certificateFile, string pinCertificate)
    {
        var user = await _context.UsersDw.FindAsync(id);

        if (user == null) 
        {
            throw new Exception("Usuario no encontrado");
        }

        if (!user.EmailVerified)
        {
            throw new Exception("El usuario aún no ha sido validado.");
        }

        using var ms = new MemoryStream();
        await certificateFile.CopyToAsync(ms);
        var fileBytes = ms.ToArray();
        string certificate = Convert.ToBase64String(fileBytes);


        var secretRequest = new UpdateSecretRequest
        {
            SecretId = user.CI,
            SecretString = certificate + ":" + pinCertificate
        };

        await _secretsManagerClient.UpdateSecretAsync(secretRequest);

        return true;
    }

    public async Task<string> SignDocument(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int page, int positionX, int positionY)
    {
        if (certificateFile == null || pdfFile == null)
        {
            throw new ArgumentException("El certificado y el pdf son requeridos.");
        }

        using (var certificateStream = certificateFile.OpenReadStream())
        using (var pdfStream = pdfFile.OpenReadStream())
        {
            var (privateKey, bcChain) = _certificateUtils.LoadCertificateChain(certificateStream, password);
            var signedPdf = _certificateUtils.SignPdfDocument(pdfStream, privateKey, bcChain, reason, location, page, positionX, positionY);

            var signedPdfBase64 = Convert.ToBase64String(signedPdf);
            return signedPdfBase64;
        }
    }

    public async Task<bool> DeleteUser(string id)
    {
        var user = await _context.UsersDw.FirstOrDefaultAsync(x => x.CI == id);
        if (user == null)
        {
            return false;
        }

        _context.UsersDw.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string> AuthentificateWithToken(string token)
    {
        var user = await _context.UsersDw.FirstOrDefaultAsync(u => u.Token == token);

        if (user == null || user.TokenExpiration < DateTime.UtcNow) 
        {
            throw new Exception("Token inválido o expirado.");
        }

        return user.Email;
    }
}
