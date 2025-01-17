using Microsoft.EntityFrameworkCore;
using Singer.Domain;
using Singer.Domain.Dtos;
using Singer.Infrastructure;
using Singer.Interfaces;

namespace Singer.Application;

public class SmtpConfigService : ISmtpConfigService
{
    private readonly SignerDbContext _context;

    public SmtpConfigService(SignerDbContext context)
    {
        _context = context;
    }

    public async Task<SmtpConfig> AddSmtpConfigAsync(SmtpConfigDto smtpConfigDto)
    {
        var smtpConfig = new SmtpConfig
        {
            Host = smtpConfigDto.Host,
            Port = smtpConfigDto.Port,
            UserName = smtpConfigDto.Username,
            Password = smtpConfigDto.Password,
            UseSsl = smtpConfigDto.UseSsl
        };

        _context.SmtpConfigs.Add(smtpConfig);
        await _context.SaveChangesAsync();

        return smtpConfig;
    }

    public async Task DeleteSmtpConfigAsync(int id)
    {
        var smtpConfig = await _context.SmtpConfigs.FindAsync(id);
        if (smtpConfig != null)
        {
            _context.SmtpConfigs.Remove(smtpConfig);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SmtpConfig>> GetAllSmtpConfigsAsync()
    {
        return await _context.SmtpConfigs.ToListAsync();
    }

    public async Task<SmtpConfig> GetSmtpConfigByIdAsync(int id)
    {
        return await _context.SmtpConfigs.FindAsync(id);
    }

    public async Task UpdateSmtpConfigAsync(int id, SmtpConfigDto smtpConfigDto)
    {
        var smtpConfig = await _context.SmtpConfigs.FindAsync(id);
        if (smtpConfig == null) throw new Exception("SMTP Config no encontrada");

        smtpConfig.Host = smtpConfigDto.Host;
        smtpConfig.Port = smtpConfigDto.Port;
        smtpConfig.UseSsl = smtpConfigDto.UseSsl;
        smtpConfig.UserName = smtpConfigDto.Username;
        smtpConfig.Password = smtpConfigDto.Password;

        _context.SmtpConfigs.Update(smtpConfig);
        await _context.SaveChangesAsync();
    }
}
