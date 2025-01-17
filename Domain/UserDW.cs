using Singer.Domain.Dtos;
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

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public UserRole Role { get; set; }

    public string ValidationPin { get; set; }
    public bool EmailVerified { get; set; }
    public virtual ICollection<DocumentDW> Documents { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiration { get; set; }

    public bool HasPendingDocuments
    {
        get
        {
            return Documents.Any(d => !d.IsSigned);
        }
    }
}
