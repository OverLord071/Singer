using System.ComponentModel.DataAnnotations;

namespace Singer.Domain;

public class UserDW
{
    [Key]
    public Guid Id { get; set; }

    public string CI { get; set; }

    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    public string Certificate { get; set; }

    public string PinCertificate { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }

    public string ValidationPin { get; set; }
    public bool EmailVerified { get; set; }
    public ICollection<DocumentDW> Documents { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiration { get; set; }
}
