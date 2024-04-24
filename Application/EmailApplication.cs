
using Microsoft.Extensions.Options;
using Singer.Domain;
using Singer.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Singer.Application;

public class EmailApplication : IMessage
{
    private readonly GmailSettings _gmailSettings;

    public EmailApplication(IOptions<GmailSettings> gmailSettings)
    {
        _gmailSettings = gmailSettings.Value;
    }

    public void SendEmail(Email request)
    {
        try
        {
            var fromEmail = _gmailSettings.UserName;
            var password = _gmailSettings.Password;

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = request.Subject;
            message.To.Add(new MailAddress(request.Recipient));
            message.Body = request.Body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("mail.digitalsolutions.com.ec")
            {
                Port = _gmailSettings.Port,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            throw new Exception($"No se pudo enviar el email: {ex.Message}");
        }
    }
}
