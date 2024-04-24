using Singer.Domain;

namespace Singer.Interfaces;

public interface IDWApplication
{
    Task<UserDW> CreateUserDW(UserDW userDW);
    Task<SignerCertificate> GetUserById(string id);
    Task<bool> CreateCertificate(string id, IFormFile certificateFile, string pinCertificate);
    Task<bool> UpdateCertificate(string id, IFormFile certificateFile, string pinCertificate);
    Task<UserDW> AuthentificateUser(string usernameOrEmail, string password);
    Task SendPinValidation(string email);
    Task ChangePassword(string email, string validationPin, string newPassword);
    Task VerifyEmail(string email, string verificationCode);
    Task<string> SignDocument(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int page, int positionX, int positionY);
    Task<bool> DeleteUser(string id);
}
