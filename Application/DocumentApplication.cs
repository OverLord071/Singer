using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Singer.Domain;
using Singer.Domain.Dtos;
using Singer.Infrastructure;
using Singer.Interfaces;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Singer.Application;

public class DocumentApplication : IDocumentApplication
{
    private readonly SignerDbContext _context;
    private readonly IMessage _emailService;

    public DocumentApplication(SignerDbContext context, IMessage emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<UserDW> CreateUserWithTemporalPassword(string email)
    {
        string temporalPassword = GeneratePassword();

        var user = new UserDW
        {
            Id = Guid.NewGuid(),
            CI = "",
            Name = "",
            Email = email,
            UserName = email,
            Role = UserRole.User,
            EmailVerified = false,
            ValidationPin = "",
            Token = GenerateToken(),
            TokenExpiration = DateTime.UtcNow.AddDays(1)
        };

        var passwordHasher = new PasswordHasher<UserDW>();
        user.Password = passwordHasher.HashPassword(user, temporalPassword);

        _context.UsersDw.Add(user);
        await _context.SaveChangesAsync();

        return user;

    }

    private string GenerateToken()
    {
        return Guid.NewGuid().ToString();
    }

    private string GeneratePassword()
    {
        var password = Path.GetRandomFileName();
        password = password.Replace(".", "").Substring(0, 8);
        return password;
    }

    public async Task<string> DownloadPdf(string urlFile, string token)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(urlFile);
        response.EnsureSuccessStatusCode();

        //var folderPath = @"G:\DocumentsSignerApp\SignerDocuments";
        var folderPath = @"C:\Users\chris\Documents\Signer";
        Directory.CreateDirectory(folderPath);

        var pathFile = Path.Combine(folderPath, Guid.NewGuid().ToString() + ".pdf");
        await File.WriteAllBytesAsync(pathFile, await response.Content.ReadAsByteArrayAsync());

        return pathFile;
    }

    public async Task<DocumentDW> SaveDocument(DocumentDto documentDto)
    {
        var existingDocument = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentDto.id_documento);
        if (existingDocument != null)
        {
            throw new Exception("El documento ya ha sido cargado.");
        }

        var token = await GetToken();
        string pathFile;
        try
        {
            pathFile = await DownloadPdf(documentDto.url_documento, token);
        }
        catch (Exception ex) 
        {
            throw new Exception($"No se descargo el documento {ex.Message}");
        }

        var userDW = await _context.UsersDw.Include(u => u.Documents).FirstOrDefaultAsync(u => u.Email == documentDto.email);
        if (userDW == null)
        {
            userDW = await CreateUserWithToken(documentDto.email, documentDto.full_name);
        }

        userDW.Token = await RefreshToken(userDW);
        
        var emailRequest = new Email
        {
            Recipient = documentDto.email,
            Subject = "Documento por firmar",
            Body = $"Tiene un documento por firmar. Por favor, acceda al siguiente enlace: <a href='https://capable-narwhal-1cc916.netlify.app/login?token={userDW.Token}'>aquí</a>"
        };
        _emailService.SendEmail(emailRequest);
        

        var document = new DocumentDW
        {
            Id = documentDto.id_documento,
            Title = Path.GetFileNameWithoutExtension(pathFile),
            IsSigned = false,
            PathFile = pathFile,
            UserId = userDW.Id,
            Email = documentDto.email,
            Section = documentDto.seccion,
            DocumentId = documentDto.id_dw_documento,
            Archiver = documentDto.archivador,
            ArchiverGuid = documentDto.guid_archivador,
            DocumentType = documentDto.tipo_documento,
            Date = documentDto.fecha,
            DocumentUrl = documentDto.url_documento,
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    private async Task<string> RefreshToken(UserDW user)
    {
        user.Token = GenerateToken();
        user.TokenExpiration = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();
        return user.Token;
    }

    private async Task<UserDW?> CreateUserWithToken(string email, string name)
    {
        string temporalPassword = GeneratePassword();

        var user = new UserDW
        {
            Id = Guid.NewGuid(),
            Email = email,
            CI = "",
            Name = name,
            UserName = "",
            Role = UserRole.User,
            EmailVerified = true,
            ValidationPin = "",
            Token = GenerateToken(),
            TokenExpiration = DateTime.UtcNow.AddDays(1)
        };

        var passwordHasher = new PasswordHasher<UserDW>();
        user.Password = passwordHasher.HashPassword(user, temporalPassword);

        _context.UsersDw.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    private async Task<string> GetToken()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://dwdemos.digitalsolutions.com.ec/dwapi2/api/Authentication/Validar"),
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                uri = "https://dwdemos.digitalsolutions.com.ec/DocuWare/Platform/",
                user = "Christian.Albarracin",
                pasw = "DS-1820",
                fid = "6e3dfea0-5b96-4a89-bd9d-b2ec86e71bb3"
            }), Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
        return tokenResponse.token;
    }

    public async Task<List<DocumentInfo>> GetDocumentsByUser(string email, Func<string, string> createDocumentUrl)
    {
        var user = await _context.UsersDw.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) 
        {
            return null;
        }

        var documents = await _context.Documents.Where(d => d.UserId == user.Id && d.IsSigned == false).ToListAsync();

        var documentInfos = new List<DocumentInfo>();
        foreach (var document in documents)
        {
            var url = createDocumentUrl(document.Id);
            documentInfos.Add(new DocumentInfo 
            {
                Id = document.Id, 
                Title = document.DocumentType,
                Url = url,
                IsSigned = document.IsSigned, 
                Date = document.Date,
                ExpirationDate = document.ExpirationDate,
                StatusDocument = document.StatusDocument
            });
        }

        return documentInfos;
    }

    public async Task<byte[]> GetDocumentFile(string id)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        if (document == null)
        {
            return null;
        }

        var bytes = await File.ReadAllBytesAsync(document.PathFile);
        return bytes;
    }

    public async Task<bool> UpdateDocumentIsSigned(string id)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        if(document == null)
        { 
            return false; 
        }

        document.IsSigned = true;
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<List<DocumentInfo>> GetAllDocuments(Func<string, string> createDocumentUrl)
    {
        var documents = await _context.Documents.ToListAsync();

        var documentInfos = new List<DocumentInfo>();

        foreach (var document in documents)
        {
            var url = createDocumentUrl(document.Id);
            documentInfos.Add(new DocumentInfo
            {
                Id = document.Id,
                Title = document.DocumentType,
                Url = url,
                IsSigned = document.IsSigned,
                Date = document.Date,
                ExpirationDate = document.ExpirationDate,
                StatusDocument = document.StatusDocument
            });
        }

        return documentInfos;
    }

    public async Task ResendEmailForSignature(string documentId)
    {
        var document = await _context.Documents
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null) 
        {
            throw new Exception("No se encontro el documento especificado.");
        }

        if (document.IsSigned)
        {
            throw new Exception("El documento ya ha sido firmado.");
        }

        var userDW = document.User;
        if (userDW == null)
        {
            throw new Exception("No se encontro el usuario relacionado con el documento.");
        }

        userDW.Token = await RefreshToken(userDW);

        var emailRequest = new Email
        {
            Recipient = document.Email,
            Subject = "Recordatorio: Documento pendiente de firma",
            Body = $"Tiene un documento pendiente de firma. Por favor, acceda al siguiente enlace para completarlo: " +
               $"<a href='https://capable-narwhal-1cc916.netlify.app/?token={userDW.Token}'>Firmar documento</a>"
        };

        _emailService.SendEmail(emailRequest);
    }

    public async Task<bool> DeleteDocument(string documentId)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document == null) 
        {
            return false;
        }

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }
}
