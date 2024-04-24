using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Singer.Utilities.Appearance;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Singer.Utilities.Certificate;

public class CertificateUtils
{
    public static string GetCN(X509Certificate2 certificate)
    {
        if (certificate == null)
        {
            return null;
        }

        return GetCN(certificate.Subject);
    }

    public static string GetCN(string principal)
    {
        if (principal == null)
        {
            return null;
        }

        string rdn = GetRDNvalueFromLdapName("CN", principal);
        if (rdn == null)
        {
            rdn = GetRDNvalueFromLdapName("OU", principal);
        }

        if (rdn != null)
        {
            return rdn;
        }

        int i = principal.IndexOf('=');
        if (i != -1)
        {
            Console.WriteLine("No se ha podido obtener el Common Name ni la Organizational Unit, se devolvera el fragmento mas significativo");
            return GetRDNvalueFromLdapName(principal.Substring(0, i), principal);
        }

        Console.WriteLine("Principal no valido, se devolvera la entrada");
        return principal;
    }

    public static List<string> GetAuthorityInformationAccess(X509Certificate2 cert)
    {
        byte[] authInfoExt = cert.Extensions["2.5.29.35"].RawData;
        Asn1OctetString octetString = Asn1OctetString.GetInstance(authInfoExt);
        Asn1Sequence objectSequence = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(octetString));
        AuthorityInformationAccess authInfo = AuthorityInformationAccess.GetInstance(objectSequence);
        List<string> ocspUrls = new List<string>();

        foreach (AccessDescription accessDescription in authInfo.GetAccessDescriptions())
        {
            GeneralName generalName = accessDescription.AccessLocation;

            if (generalName.TagNo == GeneralName.UniformResourceIdentifier)
            {
                DerIA5String str = DerIA5String.GetInstance(generalName.Name);
                string accessLocation = str.GetString();
                ocspUrls.Add(accessLocation);
            }
        }

        return ocspUrls;
    }

    public static List<string> GetCrlDistributionPoints(X509Certificate2 cert)
    {
        byte[] crldpExt = cert.Extensions["2.5.29.31"].RawData;
        CrlDistPoint distPoint = CrlDistPoint.GetInstance(crldpExt);
        List<string> crlUrls = new List<string>();

        foreach (DistributionPoint dp in distPoint.GetDistributionPoints())
        {
            DistributionPointName dpn = dp.DistributionPointName;
            if (dpn != null && dpn.PointType == DistributionPointName.FullName)
            {
                GeneralName[] generalNames = GeneralNames.GetInstance(dpn.Name).GetNames();
                foreach (GeneralName generalName in generalNames) 
                {
                    if (generalName.TagNo == GeneralName.UniformResourceIdentifier)
                    {
                        crlUrls.Add(DerIA5String.GetInstance(generalName).GetString());
                    }
                }
            }
        }

        return crlUrls;
    }

    private static string GetRDNvalueFromLdapName(string rdnType, string ldapName)
    {
        Regex regex = new Regex(string.Format(@"{0}=([^,]*)", rdnType), RegexOptions.IgnoreCase);
        Match match = regex.Match(ldapName);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
    }

    public (RsaPrivateCrtKeyParameters, ICollection<Org.BouncyCastle.X509.X509Certificate>) LoadCertificateChain(Stream certifiateStream, string password)
    {
        Pkcs12Store store = new Pkcs12Store(certifiateStream, password.ToCharArray());
        string alias = FindCertificateAlias(store);

        AsymmetricKeyEntry key = store.GetKey(alias);
        RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)key.Key;

        System.Security.Cryptography.X509Certificates.X509Certificate[] chain = store.GetCertificateChain(alias)
            .Select(entry => DotNetUtilities.ToX509Certificate(entry.Certificate))
            .ToArray();

        ICollection<Org.BouncyCastle.X509.X509Certificate> bcChain = ConvertToBouncyCastleChain(chain);

        return (privateKey, bcChain);
    }

    private string FindCertificateAlias(Pkcs12Store store) 
    {
        foreach (string currentAlias in store.Aliases)
        {
            if (store.IsKeyEntry(currentAlias))
            {
                return currentAlias;
            }
        }

        throw new ArgumentException("No valid certificate alias found in the certificate.");
    }

    private ICollection<Org.BouncyCastle.X509.X509Certificate> ConvertToBouncyCastleChain(System.Security.Cryptography.X509Certificates.X509Certificate[] chain)
    {
        ICollection<Org.BouncyCastle.X509.X509Certificate> bcChain = new List<Org.BouncyCastle.X509.X509Certificate>();
        foreach (var netCert in chain)
        {
            byte[] encoded = netCert.Export(X509ContentType.Cert);
            X509CertificateParser cp = new X509CertificateParser();
            Org.BouncyCastle.X509.X509Certificate bcCert = cp.ReadCertificate(encoded);
            bcChain.Add(bcCert);
        }

        return bcChain;
    }

    public byte[] SignPdfDocument(Stream pdfStream, RsaPrivateCrtKeyParameters privateKey, ICollection<Org.BouncyCastle.X509.X509Certificate> bcChain, string? reason, string? location, int page, int positionX, int positionY)
    {
        using (var reader = new PdfReader(pdfStream))
        using (var outputMemoryStream = new MemoryStream())
        {
            var rectangle = new Rectangle(positionX, positionY, positionX + 114, positionY + 42);
            PdfStamper stamper = PdfStamper.CreateSignature(reader, outputMemoryStream, '\0');
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;

            appearance.Reason = reason;
            appearance.Location = location;
            appearance.SetVisibleSignature(rectangle, page, null);
            string issuer = GetCN(bcChain.First().SubjectDN.ToString());
            string date = DateTime.Now.ToString("o");

            QrAppearance qrAppearance = new QrAppearance(issuer, appearance.Reason, appearance.Location, date);
            qrAppearance.CreateCustomAppearance(appearance, page, reader, rectangle);

            IExternalSignature pks = new PrivateKeySignature(privateKey, "SHA-256");
            MakeSignature.SignDetached(appearance, pks, bcChain, null, null, null, 0, CryptoStandard.CMS);

            return outputMemoryStream.ToArray();
        }
    }
}
