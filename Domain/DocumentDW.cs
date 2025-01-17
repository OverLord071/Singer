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
    public DateTime Date { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string DocumentUrl { get; set; }
    public StatusDocument StatusDocument { get; set; }
    public string? Reason { get; set; }

    [ForeignKey("UserId")]
    public UserDW User { get; set; }

    public void SetStatus(StatusDocument status, string? reason = null)
    {
        StatusDocument = status;

        if (status == StatusDocument.Rechazado)
        {
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentException("Motivo de rechazo requerido.");
            }
            Reason = reason;
        }
        else
        {
            Reason = null;
        }
    }
}
