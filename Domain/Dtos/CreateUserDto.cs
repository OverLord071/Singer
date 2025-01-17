using System.ComponentModel.DataAnnotations;

namespace Singer.Domain.Dtos;

public class CreateUserDto
{
    [Required]
    public string CI { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string UserName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }

    [Required]
    public UserRole Role { get; set; }
}
