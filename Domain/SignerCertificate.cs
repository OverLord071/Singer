using System.ComponentModel.DataAnnotations;

namespace Singer.Domain;

public class SignerCertificate
{
    public string Certificate { get; set; }
    public string PinCertificate { get; set;}
}
