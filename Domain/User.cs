using Org.BouncyCastle.Asn1;
using System.ComponentModel.DataAnnotations;

namespace Singer.Domain;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public bool IsAprroved { get; set; }
    public string EncryptionKey { get; set; }
    public string IV { get; set; }
}
