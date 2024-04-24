using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Singer.Domain;
using Singer.Interfaces;
using Singer.Utilities.Appearance;
using Singer.Utilities.Certificate;
using Singer.Utilities.KeyStore;
using System.Security.Cryptography.X509Certificates;
using Document = Singer.Domain.Document;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;


namespace Singer.Application;

public class SignerApplication: ISignerApplication
{
    private readonly CertificateUtils _certificateUtils;

    public SignerApplication()
    {
        _certificateUtils = new CertificateUtils();
    }

    public async Task<Certificate> ValidateCertificate(IFormFile certificateFile, string password) 
    {
        if (certificateFile == null || string.IsNullOrEmpty(password))
            throw new ArgumentException("Se requieren los archivos del certificado y la contraseña.");
        try
        {
            using (MemoryStream certificateStream = new MemoryStream())
            {
                await certificateFile.CopyToAsync(certificateStream);

                var keyStore = GetKeyStore(certificateStream, password, null);
                string alias = SelectAlias(keyStore);

                X509Certificate2 x509Certificate = keyStore.Certificates.Find(X509FindType.FindBySubjectName, alias, false)
                    .OfType<X509Certificate2>()
                    .FirstOrDefault();

                if (x509Certificate == null)
                {
                    throw new Exception($"No se pudo encontrar el certificado correspondiente {alias}");
                }

                X509Chain chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;

                bool isChainValid = chain.Build(x509Certificate);
                bool isRevoked = false;
                if (!isChainValid) 
                {
                    foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                    {
                        if (chainStatus.Status == X509ChainStatusFlags.Revoked)
                        {
                            await Console.Out.WriteLineAsync("El certificado ha sido revocado");
                            isRevoked = true;
                            break;
                        }
                        
                    }
                }

                Certificate certificate = new Certificate
                {
                    IssuedTo = CertificateUtils.GetCN(x509Certificate.Issuer),
                    IssuedBy = CertificateUtils.GetCN(x509Certificate.Subject),
                    ValidFrom = x509Certificate.NotBefore,
                    ValidTo = x509Certificate.NotAfter,
                    Generated = DateTime.Now,
                    //Revocated = ValidateDateRevoked(x509Certificate, null),
                    Validated = !isRevoked
                };

                return certificate;
            }
        }
        catch (Exception ex) 
        {
            throw new Exception($"Error al verificar el certificado. {ex.Message}");
        }
    }

    private DateTime? ValidateDateRevoked(X509Certificate2 x509Certificate, object value)
    {
        throw new NotImplementedException();
    }

    private string SelectAlias(X509Store keyStore)
    {
        var signingAliases = KeyStoreUtilities.getSigningAliases(keyStore);

        if (signingAliases.Count == 0) 
        {
            throw new Exception("No se encuentra certificados ");
        }
        else if (signingAliases.Count == 2) 
        {
            return signingAliases[0].Name;
        }
        else 
        {
            return null;
        }

    }

    private X509Store GetKeyStore(MemoryStream certificateFile, string password, string tipeKeyStore)
    {
        if (certificateFile != null)
        {
            try
            {
                certificateFile.Seek(0, SeekOrigin.Begin);

                var keyStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                keyStore.Open(OpenFlags.ReadWrite);

                var certCollection = new X509Certificate2Collection();
                certCollection.Import(certificateFile.ToArray(), password, X509KeyStorageFlags.PersistKeySet);
                keyStore.AddRange(certCollection);

                return keyStore;
                
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("El archivo está nulo o vacío.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar el archivo {ex.Message}");
            }
        }
        else
        {
            throw new ArgumentException("El archivo esta nulo o vacio");
        }
    }

    public async Task<byte[]> SignPdfDocumentAsync(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int page, int positionX, int positionY)
    {
        if (certificateFile == null || pdfFile == null)
        {
            throw new ArgumentException("Both certificateFile and pdfFile must be provided.");
        }

        using (var certificateStream = certificateFile.OpenReadStream())
        using (var pdfStream = pdfFile.OpenReadStream())
        {
            var (privateKey, bcChain) = _certificateUtils.LoadCertificateChain(certificateStream, password);

            return _certificateUtils.SignPdfDocument(pdfStream, privateKey, bcChain, reason, location, page, positionX, positionY);
        }
    }


    public async Task<byte[]> SignPdfDocumentAsync2(IFormFile certificateFile, string password, IFormFile pdfFile, string? reason, string? location, int positionX, int positionY)
    {
        if (certificateFile == null || pdfFile == null)
        {
            throw new ArgumentException("Both certificateFile and pdfFile must be provided.");
        }

        using (var certificateStream = certificateFile.OpenReadStream())
        using (var pdfStream = pdfFile.OpenReadStream())
        {
            Pkcs12Store store = new Pkcs12Store(certificateStream, password.ToCharArray());

            string alias = null;
            foreach (string currentAlias in store.Aliases)
            {
                if (store.IsKeyEntry(currentAlias))
                {
                    alias = currentAlias;
                    break;
                }
            }

            if (alias == null)
            {
                throw new ArgumentException("No valid certificate alias found in the .p12 file.");
            }

            AsymmetricKeyEntry key = store.GetKey(alias);
            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)key.Key;

            X509Certificate[] chain = store.GetCertificateChain(alias)
                                  .Select(entry => DotNetUtilities.ToX509Certificate(entry.Certificate))
                                  .ToArray();

            ICollection<Org.BouncyCastle.X509.X509Certificate> bcChain = new List<Org.BouncyCastle.X509.X509Certificate>();
            foreach (var netCert in chain)
            {
                byte[] encoded = netCert.Export(X509ContentType.Cert);
                X509CertificateParser cp = new X509CertificateParser();
                Org.BouncyCastle.X509.X509Certificate bcCert = cp.ReadCertificate(encoded);
                bcChain.Add(bcCert);
            }

            using (var reader = new PdfReader(pdfStream))
            using (var outputMemoryStream = new MemoryStream())
            {
                var rectangle = new Rectangle(positionX, positionY, positionX + 114, positionY + 42);
                PdfStamper stamper = PdfStamper.CreateSignature(reader, outputMemoryStream, '\0');
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;

                appearance.Reason = reason;
                appearance.Location = location;
                appearance.SetVisibleSignature(rectangle, 1, null);
                string issuer = CertificateUtils.GetCN(chain[0].Subject);
                string date = DateTime.Now.ToString("o");

                QrAppearance qrAppearance = new QrAppearance(issuer, appearance.Reason, appearance.Location, date);
                qrAppearance.CreateCustomAppearance(appearance, 1, reader, rectangle);

                IExternalSignature pks = new PrivateKeySignature(privateKey, "SHA-256");
                MakeSignature.SignDetached(appearance, pks, bcChain, null, null, null, 0, CryptoStandard.CMS);

                return outputMemoryStream.ToArray();
            }
        }
    }

    public Task<Document> VerifyPdfSignature(IFormFile pdfFile)
    {
        Document document = new Document();
        try
        {
            using (var pdfStream = pdfFile.OpenReadStream())
            using (PdfReader reader = new PdfReader(pdfStream))
            {
                AcroFields fields = reader.AcroFields;
                var names = fields.GetSignatureNames();

                foreach (var name in names) 
                {
                    var pkcs7 = fields.VerifySignature(name);
                    bool isValid = pkcs7.Verify();
                    if (isValid)
                    {
                        document.SignValidate = true;
                        document.DocValidate = true;
                        
                    }
                    else 
                    {
                        document.SignValidate = false;
                        document.DocValidate = false;
                        document.Error = "La firma no es válida.";
                        return Task.FromResult(document);
                    }
                }
            }

            if (!document.SignValidate) 
            {
                document.Error = "No se encontró ninguna firma en el documento.";
            }
        }
        catch (Exception ex) 
        {
            document.Error = $"Se produjo un error al validar la firma: {ex.Message}";
        }
        return Task.FromResult(document);
    }
}
