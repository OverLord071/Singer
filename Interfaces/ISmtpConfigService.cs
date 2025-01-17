using Singer.Domain;
using Singer.Domain.Dtos;

namespace Singer.Interfaces;

public interface ISmtpConfigService
{
    Task<SmtpConfig> AddSmtpConfigAsync(SmtpConfigDto smtpConfigDto);
    Task<SmtpConfig> GetSmtpConfigByIdAsync(int id);
    Task<IEnumerable<SmtpConfig>> GetAllSmtpConfigsAsync();
    Task UpdateSmtpConfigAsync(int id, SmtpConfigDto smtpConfigDto);
    Task DeleteSmtpConfigAsync(int id);
}
