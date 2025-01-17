
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Singer.Domain;
using Singer.Infrastructure;
using Singer.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Singer.Application;

public class EmailApplication : IMessage
{
    private readonly SignerDbContext _context;

    public EmailApplication(SignerDbContext context)
    {
        _context = context;
    }

    public async void SendEmail(Email request)
    {
        try
        {
            var smtpConfig = _context.SmtpConfigs.FirstOrDefault();

            if (smtpConfig == null)
            {
                throw new Exception("No se encontró configuracion smtp");
            }

            var fromEmail = smtpConfig.UserName;
            var password = smtpConfig.Password;

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(request.Recipient));

            var smtpClient = new SmtpClient(smtpConfig.Host)
            {
                Port = smtpConfig.Port,
                Credentials = new NetworkCredential(fromEmail, password),
            };

            switch (smtpConfig.Port)
            {
                case 465:
                    smtpClient.EnableSsl = true;
                    break;
                case 587:
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    break;
                case 26:
                    smtpClient.EnableSsl = false;
                    break;
                default:
                    throw new Exception($"Puerto SMTP no valido {smtpConfig.Port}");
            }

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new Exception($"No se pudo enviar el email: {ex.Message}");
        }
    }
}
