using Singer.Domain;
using Singer.Domain.Dtos;

namespace Singer.Interfaces;

public interface IDWApplication
{
    Task<UserDW> CreateUserDW(CreateUserDto createUserDto);
    Task<SignerCertificate> GetUserById(string id);
    Task<bool> CreateCertificate(string id, IFormFile certificateFile, string pinCertificate);
    Task<bool> UpdateCertificate(string id, IFormFile certificateFile, string pinCertificate);
    Task<UserDW> AuthentificateUser(string usernameOrEmail, string password);
    Task<string> AuthentificateWithToken(string token);
    Task SendPinValidation(string email);
    Task ChangePassword(string email, string validationPin, string newPassword);
    Task VerifyEmail(string email, string verificationCode);
    Task<string> SignDocument(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int page, int positionX, int positionY);
    Task<bool> DeleteUser(string id);
    Task RejectDocumentAsync(string documentId, string reason);
    Task<bool> VerifyCode(string email, string code);
    Task<UserDW> ChangeStatusUser(Guid id);
    Task<List<UserDWDto>> GetAllUsers ();
    Task SendLinkRecoveryPassword(string email);
    Task ValidateToken(string token);
    Task ChangePasswordWithToken(string token, string password);
}
