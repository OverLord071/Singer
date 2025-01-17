using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Singer.Domain.Dtos;
using Singer.Interfaces;

namespace Singer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmtpConfigController : ControllerBase
    {
        private readonly ISmtpConfigService _smtpConfigService;

        public SmtpConfigController(ISmtpConfigService smtpConfigService)
        {
            _smtpConfigService = smtpConfigService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSmtpConfig([FromBody] SmtpConfigDto smtpConfigDto)
        {
            var createdConfig = await _smtpConfigService.AddSmtpConfigAsync(smtpConfigDto);
            return CreatedAtAction(nameof(GetSmtpConfigById), new { id = createdConfig.Id }, createdConfig);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSmtpConfigById(int id)
        {
            var smtpConfig = await _smtpConfigService.GetSmtpConfigByIdAsync(id);
            if (smtpConfig == null)
            {
                return NotFound();
            }
            return Ok(smtpConfig);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSmtpConfigs()
        {
            var smtpConfigs = await _smtpConfigService.GetAllSmtpConfigsAsync();
            return Ok(smtpConfigs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSmtpConfig(int id, [FromBody] SmtpConfigDto smtpConfigDto)
        {
            await _smtpConfigService.UpdateSmtpConfigAsync(id, smtpConfigDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSmtpConfig(int id)
        {
            await _smtpConfigService.DeleteSmtpConfigAsync(id);
            return NoContent();
        }

    }
}
