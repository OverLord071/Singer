using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Singer.Domain;

public class DocumentDW
{
    [Key]
    public string Id { get; set; }
    public string Title { get; set; }
    public bool IsSigned { get; set; }
    public string PathFile { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Section { get; set; }
    public string DocumentId { get; set; }
    public string Archiver { get; set; }
    public string ArchiverGuid { get; set; }
    public string DocumentType { get; set; }
    public string Date { get; set; }
    public string DocumentUrl { get; set; }

    [ForeignKey("UserId")]
    public UserDW User { get; set; }

}
