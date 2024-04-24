using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Security.Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Singer.Utilities.EcuadorEntities.BancoCentral;

public class BceCaCert : X509Certificate2
{
    private X509Certificate2 certificate;

    public BceCaCert() : base()
    {
        StringBuilder cer = new StringBuilder();
        cer.AppendLine("-----BEGIN CERTIFICATE-----");
        cer.AppendLine("MIIJMzCCBxugAwIBAgIETj/6bTANBgkqhkiG9w0BAQsFADCBwjELMAkGA1UEBhMC");
        cer.AppendLine("RUMxIjAgBgNVBAoTGUJBTkNPIENFTlRSQUwgREVMIEVDVUFET1IxNzA1BgNVBAsT");
        cer.AppendLine("LkVOVElEQUQgREUgQ0VSVElGSUNBQ0lPTiBERSBJTkZPUk1BQ0lPTi1FQ0lCQ0Ux");
        cer.AppendLine("DjAMBgNVBAcTBVFVSVRPMUYwRAYDVQQDEz1BVVRPUklEQUQgREUgQ0VSVElGSUNB");
        cer.AppendLine("Q0lPTiBSQUlaIERFTCBCQU5DTyBDRU5UUkFMIERFTCBFQ1VBRE9SMB4XDTExMDgw");
        cer.AppendLine("ODE0MzIwNVoXDTMxMDgwODE1MDIwNVowgcIxCzAJBgNVBAYTAkVDMSIwIAYDVQQK");
        cer.AppendLine("ExlCQU5DTyBDRU5UUkFMIERFTCBFQ1VBRE9SMTcwNQYDVQQLEy5FTlRJREFEIERF");
        cer.AppendLine("IENFUlRJRklDQUNJT04gREUgSU5GT1JNQUNJT04tRUNJQkNFMQ4wDAYDVQQHEwVR");
        cer.AppendLine("VUlUTzFGMEQGA1UEAxM9QVVUT1JJREFEIERFIENFUlRJRklDQUNJT04gUkFJWiBE");
        cer.AppendLine("RUwgQkFOQ08gQ0VOVFJBTCBERUwgRUNVQURPUjCCAiIwDQYJKoZIhvcNAQEBBQAD");
        cer.AppendLine("ggIPADCCAgoCggIBALw9wH7DgFMR3kHUp72Wpug1N8JWFRthnhqxMWxOXVnGoYbG");
        cer.AppendLine("sdVTaycXSeVnWt03AZDGw8x7FNh3A2Hh9vtOZGnFCOWJZyDqF5KiGHN6Jiy1mAD4");
        cer.AppendLine("qAgFghWCh78OBO19ThI3PAflevMwqnWF5DJsqBdV8lqvOh8L5DX54PDYcs2zXlBI");
        cer.AppendLine("76hz/Ye4BXI1dMSmlKbAVaiBMMG+Ye/szAL4RQCZNpyi65nbgXKztbvWjwJiJIbW");
        cer.AppendLine("KND9Cu40+wZ6tm+OcTKyNQfhvdSfqRZ7tQv2LDwhPotuztyS6RljyMyNe1l3A6hW");
        cer.AppendLine("D/JnS65gHi46H0WjrRqtH5ObqhTEwZszOPdU32VFcLhUtZhPQp0M74Wa2dXy9d+s");
        cer.AppendLine("DBCdI9GZcaY+nzaNMbPEdT5lFg1Uc6ksWbWvj5udZMBhygZj1PtaWFjmqpZcdd9v");
        cer.AppendLine("Z29GGbOtKB6bx162YGaK5sGjB385WVDRAi6Uzjl+0CpoDJjP7YS9tZrXlDs4gepp");
        cer.AppendLine("KETthU2cpk73jYflzBeFFavuxNHGk6cVNgFrrhht0X0/eMhgq0Go4NUyY11g/r7f");
        cer.AppendLine("3Upf0YR7OxOacjDbLpIbNxzeH2htcD0zpyS485TWnBnarjBhgO1ywQmRQ/Ryl8Zq");
        cer.AppendLine("u7eWKBOfk++hibqJNfeLwEY3uBGoITbTXpBiX2u6U86bRGHES0Cm6mud5xErAgMB");
        cer.AppendLine("AAGjggMtMIIDKTB8BgNVHSAEdTBzMHEGCisGAQQBgqg7AQEwYzBhBggrBgEFBQcC");
        cer.AppendLine("ARZVaHR0cDovL3d3dy5lY2kuYmNlLmVjL2F1dG9yaWRhZC1jZXJ0aWZpY2FjaW9u");
        cer.AppendLine("L2RlY2xhcmFjaW9uLXByYWN0aWNhcy1jZXJ0aWZpY2FjaW9uLnBkZjARBglghkgB");
        cer.AppendLine("hvhCAQEEBAMCAAcwggHtBgNVHR8EggHkMIIB4DCCAdygggHYoIIB1KSB1DCB0TEL");
        cer.AppendLine("MAkGA1UEBhMCRUMxIjAgBgNVBAoTGUJBTkNPIENFTlRSQUwgREVMIEVDVUFET1Ix");
        cer.AppendLine("NzA1BgNVBAsTLkVOVElEQUQgREUgQ0VSVElGSUNBQ0lPTiBERSBJTkZPUk1BQ0lP");
        cer.AppendLine("Ti1FQ0lCQ0UxDjAMBgNVBAcTBVFVSVRPMUYwRAYDVQQDEz1BVVRPUklEQUQgREUg");
        cer.AppendLine("Q0VSVElGSUNBQ0lPTiBSQUlaIERFTCBCQU5DTyBDRU5UUkFMIERFTCBFQ1VBRE9S");
        cer.AppendLine("MQ0wCwYDVQQDEwRDUkwxhoH6bGRhcDovL2JjZXFsZGFwcmFpenAuYmNlLmVjL2Nu");
        cer.AppendLine("PUNSTDEsY249QVVUT1JJREFEJTIwREUlMjBDRVJUSUZJQ0FDSU9OJTIwUkFJWiUy");
        cer.AppendLine("MERFTCUyMEJBTkNPJTIwQ0VOVFJBTCUyMERFTCUyMEVDVUFET1IsbD1RVUlUTyxv");
        cer.AppendLine("dT1FTlRJREFEJTIwREUlMjBDRVJUSUZJQ0FDSU9OJTIwREUlMjBJTkZPUk1BQ0lP");
        cer.AppendLine("Ti1FQ0lCQ0Usbz1CQU5DTyUyMENFTlRSQUwlMjBERUwlMjBFQ1VBRE9SLGM9RUM/");
        cer.AppendLine("YXV0aG9yaXR5UmV2b2NhdGlvbkxpc3Q/YmFzZTArBgNVHRAEJDAigA8yMDExMDgw");
        cer.AppendLine("ODE0MzIwNVqBDzIwMzEwODA4MTUwMjA1WjALBgNVHQ8EBAMCAQYwHwYDVR0jBBgw");
        cer.AppendLine("FoAUqBAVqN+gmczo6M/ubUbv6hbSCswwHQYDVR0OBBYEFKgQFajfoJnM6OjP7m1G");
        cer.AppendLine("7+oW0grMMAwGA1UdEwQFMAMBAf8wHQYJKoZIhvZ9B0EABBAwDhsIVjguMDo0LjAD");
        cer.AppendLine("AgSQMA0GCSqGSIb3DQEBCwUAA4ICAQCt5F5DaFGcZqrQ5uKKrk2D1KD2DlNbniaK");
        cer.AppendLine("IwJfZ36tLYUuyu7VmLZZdrVKqjC+FYAZIQJn/q2w/0JN5I5YK+Yj1UEa9nlmshRH");
        cer.AppendLine("aCEJXZokLXFjD4ZayiZgJh7OcMEV7G9VFFP2WF4iDflSG0drhn152Fllh+y1ZHov");
        cer.AppendLine("hX6TlCT0y5iAq+zzq2Utu6Gs1SU5U7fCC7gNYOeztPehqlnSTaD1xAbqnTVOBS1Z");
        cer.AppendLine("hoCQio5vF98TS36ItfjDA0bO12FiJKOLx9WNiimDxy0KIFSfifD1FfmUO5MYgcke");
        cer.AppendLine("CTLnkGHtCadhpEsy6HwHeeuLNPkp5DMGJeBX1XAjVC50ulw36lXuryJ9/FRBpbdg");
        cer.AppendLine("uLJIssFndQlr6klA5LdK44yFVr3+1d+59fNuiFQnKQV7bFQfApv5FqvqyfNEEI1K");
        cer.AppendLine("1prM82aq24xDT5OwsyRnf+F7p6OwQTYmGYkrGH5RlqFI+XC8ckMip3XecFb6Qyur");
        cer.AppendLine("kaA/286eYUOZiJpPgn/qlisNreF0GTLi9tBzExGCD+BdsYGqMu/gx8lgMbF3b+HK");
        cer.AppendLine("eQe8+kExkb7LVYhbTlOBZzB0da/cDmvg1V+pgrXu0qUX/YnQyybnA9nyQdLj60/3");
        cer.AppendLine("sUlWyxURbu33kTNnrPJmcHjRa561Co84NYKifLrDSgAChLQry/eItvhzFYu33Td9");
        cer.AppendLine("TkHa++TQjg==");
        cer.AppendLine("-----END CERTIFICATE-----");

        string certData = cer.ToString();

        int startIndex = certData.IndexOf("-----BEGIN CERTIFICATE-----") + "-----BEGIN CERTIFICATE-----".Length;
        int endIndex = certData.IndexOf("-----END CERTIFICATE-----", startIndex);

        string certContent = certData.Substring(startIndex, endIndex - startIndex);

        try
        {
            byte[] certBytes = Encoding.UTF8.GetBytes(certContent);
            using (MemoryStream stream = new MemoryStream(certBytes))
            {
                byte[] certDataBytes = new byte[stream.Length];
                stream.Read(certDataBytes, 0, (int)stream.Length);
                X509Certificate2 cert = new X509Certificate2(certDataBytes);
                this.certificate = cert;
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error al cargar el certificado ", ex);
        }
    }

    public void CheckValidity()
    {
        if(DateTime.Now < this.certificate.NotBefore || DateTime.Now > this.certificate.NotAfter)
        {
            throw new CertificateExpiredException("El certificado ha caducado o no es aún válido.");
        }
    }

    public void CheckValidity(DateTime date)
    {
        if (date < this.certificate.NotBefore || date > this.certificate.NotAfter)
        {
            throw new CertificateExpiredException("El certificado ha caducado o no es aún válido.");
        }
    }

    public int GetBasicConstraints()
    {
        var extension = this.certificate.Extensions["2.5.29.19"] as X509BasicConstraintsExtension;
        return extension == null ? -1 : extension.CertificateAuthority ? extension.PathLengthConstraint : -1;
    }

    public string GetIssuerDN()
    {
        return this.certificate.IssuerName.Name;
    }

    public byte[] GetIssuerUniqueID()
    {
        var extension = this.certificate.Extensions
            .OfType<X509Extension>()
            .FirstOrDefault(ext => ext.Oid.Value == "2.5.29.14");

        if (extension != null)
        {
            return extension.RawData;
        }

        return null;
    }

    public bool[] GetKeyUsage()
    {
        var extension = this.certificate.Extensions["2.5.29.15"] as X509KeyUsageExtension;
        return extension == null ? null : ConvertToBoolArray(extension.KeyUsages);
    }

    private bool[] ConvertToBoolArray(X509KeyUsageFlags flags)
    {
        bool[] result = new bool[8];
        result[0] = (flags & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature;
        result[1] = (flags & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation;
        result[2] = (flags & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment;
        result[3] = (flags & X509KeyUsageFlags.DataEncipherment) == X509KeyUsageFlags.DataEncipherment;
        result[4] = (flags & X509KeyUsageFlags.KeyAgreement) == X509KeyUsageFlags.KeyAgreement;
        result[5] = (flags & X509KeyUsageFlags.KeyCertSign) == X509KeyUsageFlags.KeyCertSign;
        result[6] = (flags & X509KeyUsageFlags.CrlSign) == X509KeyUsageFlags.CrlSign;
        result[7] = (flags & X509KeyUsageFlags.EncipherOnly) == X509KeyUsageFlags.EncipherOnly;
        return result;
    }

    public DateTime GetNotAfter()
    {
        return this.certificate.NotAfter;
    }

    public DateTime GetNotBefore()
    {
        return this.certificate.NotBefore;
    }

    public string GetSerialNumber()
    {
        return this.certificate.SerialNumber;
    }

    public string GetSigAlgName()
    {
        return this.certificate.SignatureAlgorithm.FriendlyName;
    }

    public string GetSigAlgOID()
    {
        return this.certificate.SignatureAlgorithm.Value;
    }

    public string GetSigAlgParams()
    {
        return this.certificate.SignatureAlgorithm.FriendlyName;
    }

    public byte[] GetSignature()
    {
        return this.certificate.GetRawCertData();
    }

    public string GetSubjectDN()
    {
        return this.certificate.Subject;
    }

    public byte[] GetTBSCertificate()
    {
        return this.certificate.RawData;
    }

    public int GetVersion()
    {
        return this.certificate.Version;
    }

    public byte[] GetEncoded()
    {
        return this.certificate.Export(X509ContentType.Cert);
    }

    public PublicKey GetPublicKey()
    {
        return this.certificate.PublicKey;
    }

    public override string ToString()
    {
        return this.certificate.ToString();
    }

    public void Verify()
    {
        this.certificate.Verify();
    }

    public bool Verify(X509Certificate2 issuerCert)
    {
        if (this.Issuer == issuerCert.Subject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string[] GetCriticalExtensionOIDs()
    {
        var criticalExtensions = this.certificate.Extensions;
        var result = new string[criticalExtensions.Count];
        int index = 0;
        foreach (var extension in criticalExtensions)
        {
            if (extension.Critical)
            {
                result[index++] = extension.Oid.Value;
            }
        }
        return result;
    }

    public byte[] GetExtensionValue(string oid)
    {
        var extension = this.certificate.Extensions[oid];
        if (extension != null)
        {
            return extension.RawData;
        }
        return null;
    }

    public string[] GetNonCriticalExtensionOIDs()
    {
        var nonCriticalExtensions = this.certificate.Extensions;
        var result = new string[nonCriticalExtensions.Count];
        int index = 0;
        foreach (var extension in nonCriticalExtensions)
        {
            if (!extension.Critical)
            {
                result[index++] = extension.Oid.Value;
            }
        }
        return result;
    }

    public bool HasUnsupportedCriticalExtension()
    {
        var extensions = this.certificate.Extensions;
        foreach (var extension in extensions)
        {
            if (extension.Critical && !IsKnownExtension(extension.Oid.Value))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsKnownExtension(string oid)
    {
        // Aquí puedes agregar lógica para identificar extensiones conocidas
        // Por ejemplo, si tienes una lista de extensiones conocidas, puedes verificar si el OID está en esa lista.
        return false;
    }
}
