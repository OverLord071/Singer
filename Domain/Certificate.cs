namespace Singer.Domain;

public class Certificate
{
    public string IssuedTo { get; set; }
    public string IssuedBy { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public DateTime Generated { get; set; }
    public DateTime? Revocated { get; set; }
    public bool Validated { get; set; }
    public string KeyUsages { get; set; }
    public UserCertificate User { get; set; }
    public bool SignVerify { get; set; }
    public DateTime? DocTimeStamp { get; set; }
    public string DocReason { get; set; }
    public string DocLocation { get; set; }

    public override string ToString()
    {
        return $@"
        Certificado
        [issuedTo={IssuedTo}
        issuedBy={IssuedBy}
        validFrom={(ValidFrom == null ? null : ValidFrom.ToString("yyyy-MM-dd HH:mm:ss"))}
        validTo={(ValidTo == null ? null : ValidTo.ToString("yyyy-MM-dd HH:mm:ss"))}
        generated={(Generated == null ? null : Generated.ToString("yyyy-MM-dd HH:mm:ss"))}
        revocated={(Revocated == null ? null : Revocated.Value.ToString("yyyy-MM-dd HH:mm:ss"))}
        validated={Validated}
        keyUsages={KeyUsages}
        docTimeStamp={(DocTimeStamp == null ? null : DocTimeStamp.Value.ToString("yyyy-MM-dd HH:mm:ss"))}
        signVerify={SignVerify}
        docReason={DocReason}
        docLocation={DocLocation}
        {(User == null ? "\tUserData[Sin información de usuario]" : User.ToString())}
        ]";
    }
}
