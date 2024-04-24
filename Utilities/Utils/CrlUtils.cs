using Org.BouncyCastle.Utilities.Zlib;
using Singer.Utilities.Certificate;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.Utils;

public class CrlUtils
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static string revocationDate;
    public CrlUtils()
    {
    }
    public string GetRevocationDate()
    {
        return revocationDate;
    }

    public static ValidationResult VerifyCertificateCRLs(X509Certificate2 cert, PublicKey publicKey, List<string> overridingDistributionPoints)
    {
        if (cert == null) 
        {
            return ValidationResult.Corrupt;
        }

        List<string> crlDistPoints = new List<string>();

        try
        {
            var ocspUrls = CertificateUtils.GetAuthorityInformationAccess(cert);
            foreach (var ocsp in ocspUrls)
            {
                Console.WriteLine();
            }

            Console.WriteLine("Valid? " + OcspUtils.IsValidCertificate(cert));
            crlDistPoints = overridingDistributionPoints == null || overridingDistributionPoints.Count == 0
                ? CertificateUtils.GetCrlDistributionPoints(cert) : overridingDistributionPoints;
        }
        catch (IOException e)
        {
            Logger.Error("Error obteniedo los puntos de distribucion de CRL: " + e);
        }

        Logger.Info("El certificado con serie '" + cert.SerialNumber + "' tiene asociadas las sisguientes CRL: " + string.Join(", ", crlDistPoints));
        var cf = new X509Certificate2();
        bool checkedCrl = false;
        bool cannotDownload = false;

        foreach (var crlDP in crlDistPoints) 
        {
            if (crlDP.ToLower().Contains("ocsp"))
            {
                continue;
            }

            byte[] crlBytes;

            try
            {
                crlBytes = DownloadCRL(crlDP);
            }
            catch (Exception e)
            {
                Logger.Error("No se ha podido descargar el CRL (" + crlDP + "), se continurara con el siguiente punto de distribucion: " + e);
                cannotDownload = true;
                continue;
            }

            X509Certificate2 crl;

            try
            {
                crl = new X509Certificate2(crlBytes);
                var entry = crl.NotBefore;
                revocationDate = entry.ToString();
            }
            catch (Exception e)
            {
                Logger.Warn("Error anlaizando la lista de revocacion: " + e);
                crl = new X509Certificate2();
            }

            if (crl.HasPrivateKey)
            {
                try
                {
                    crl.Verify();
                }
                catch (Exception e) 
                {
                    Logger.Error("No se ha podido comprobar la firma de la CRL: " + e);
                    return ValidationResult.ServerError;
                }
            }

            checkedCrl = true;
        }

        if (checkedCrl) 
        {
            return ValidationResult.Valid;
        }

        if (cannotDownload)
        {
            return ValidationResult.CannotDownloadCrl;
        }

        return ValidationResult.Unknown;

    }

    private static byte[] DownloadCRL(string crlURL)
    {
        if (crlURL.StartsWith("http://") || crlURL.StartsWith("https://"))
        {
            return DownloadCRLFromWeb(crlURL);
        }
        else if (crlURL.StartsWith("ldap://"))
        {
            return DownloadCRLFromLDAP(crlURL);
        }
        else 
        {
            throw new Exception("No se soporta el protocola del punto de distribucion de CRL: " + crlURL); ;
        }
    }

    private static byte[] DownloadCRLFromLDAP(string url)
    {
        var val = new byte[] { };

        var request = WebRequest.Create(url);
        request.Method = "GET";

        using (var response = request.GetResponse())
        using (var stream = response.GetResponseStream())
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            val = memoryStream.ToArray();
        }

        if (val == null || val.Length == 0)
        {
            throw new Exception("No se ha podido descargar la CRL desde " + url);
        }
        else 
        {
            return val;
        }
    }

    private static byte[] DownloadCRLFromWeb(string url)
    {
        using (var httpCLient = new HttpClient())
        {
            return httpCLient.GetByteArrayAsync(url).Result;
        }
    }

    public enum ValidationResult
    {
        Valid,
        Revoked,
        Corrupt,
        ServerError,
        CannotDownloadCrl,
        Unknown
    }
}
