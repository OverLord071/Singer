using Singer.Domain;
using Singer.Domain.Dtos;

namespace Singer.Interfaces;

public interface IDocumentApplication
{
    Task<DocumentDW> SaveDocument(DocumentDto documentDto);
    Task<UserDW> CreateUserWithTemporalPassword(string email);
    Task<string> DownloadPdf(string urlFile, string token);
    Task<List<DocumentInfo>> GetDocumentsByUser(string email, Func<string, string> createDocumentUrl);
    Task<byte[]> GetDocumentFile(string id);
    Task<bool> UpdateDocumentIsSigned(string id);
}
