using Singer.Domain;

namespace Singer.Interfaces;

public interface ISignerApplication
{
    Task<Certificate> ValidateCertificate(IFormFile certificateFile, string password);
    Task<byte[]> SignPdfDocumentAsync(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int page, int positionX, int positionY);
    Task<byte[]> SignPdfDocumentAsync2(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int positionX, int positionY);
    Task<Document> VerifyPdfSignature(IFormFile pdfFile);
}
