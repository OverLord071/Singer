namespace Singer.Domain;

public class Document
{
    public bool SignValidate { get; set; }
    public bool DocValidate { get; set; }
    public List<Certificate> Certificates { get; set; }
    public string Error { get; set; }

    public Document()
    {
        Certificates = new List<Certificate>();
    }

    public Document(bool signValidate, bool docValidate, List<Certificate> certificates, string error)
    {
        SignValidate = signValidate;
        DocValidate = docValidate;
        Certificates = certificates ?? new List<Certificate>();
        Error = error;
    }

    public override string ToString()
    {
        return $"\tDocument\n" +
               $"\t[signValidate={SignValidate}\n" +
               $"\tdocValidate={DocValidate}\n" +
               $"\terror={Error}\n" +
               $"\t]";
    }
}
