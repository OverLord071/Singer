using Singer.Utilities.Certificate;
using Singer.Utilities.OCSP;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using static Singer.Utilities.Utils.CrlUtils;

namespace Singer.Utilities.Utils;

public class UtilsCrlOcsp
{
    public static async Task<string?> ValidateCertificateAsync(X509Certificate2 cert, string apiUrl)
    {
        string? revokedDate = null;
        try
        {
            BigInteger serial = int.Parse(cert.GetSerialNumber().ToString());
            revokedDate = await ValidateCrlServerApiAsync(serial, apiUrl);
        }
        catch (SocketException ex)
        {
            await Console.Out.WriteLineAsync($"Socket Exception: {ex.Message}");
            revokedDate = "errorRed";
        }
        catch (IOException)
        {
            await Console.Out.WriteLineAsync("Validation failed through API service, Now trying via OCSP");
            try
            {
                revokedDate = await ValidateOCSPAsync(cert);
                if (revokedDate == "unknownStatus") 
                {
                    revokedDate = null;
                }
            }
            catch (IOException)
            {
                await Console.Out.WriteLineAsync("Validation failed through OCSP, Now trying via CRL");
                revokedDate = await ValidateCRLAsync(cert);
            }
        }

        return revokedDate;
    }

    private static async Task<string?> ValidateCRLAsync(X509Certificate2 cert)
    {
        X509Certificate2 root = CertificateEcuUtils.GetRootCertificate(cert);
        CrlUtils crlUtils = new CrlUtils();
        string? crlUrl = GetCRLUrl(CertificateUtils.GetCrlDistributionPoints(cert));
        ValidationResult result = VerifyCertificateCRLs(cert, root.PublicKey, new List<string> { crlUrl });

        if (result == ValidationResult.CannotDownloadCrl)
        {
            throw new Exception($"Unable to validate against the revocation list: {crlUrl}");
        }
        if (result != ValidationResult.Valid)
        {
            throw new Exception("Invalid Certificate");
        }

        return crlUtils.GetRevocationDate();
    }

    private static string? GetCRLUrl(List<string> list)
    {
        foreach (var url in list) 
        {
            if (url.ToLower().Contains("crl"))
            {
                return url;
            }
        }

        return null;
    }

    private static async Task<string?> ValidateOCSPAsync(X509Certificate2 cert)
    {
        List<string> ocspUrls = CertificateUtils.GetAuthorityInformationAccess(cert);
        foreach (var ocspUrl in ocspUrls)
        {
            await Console.Out.WriteLineAsync("OCSP=" + ocspUrl);
        }

        X509Certificate2 certRoot = CertificateEcuUtils.GetRootCertificate(cert);
        
        var result = ValidatorOCSP.ValidarOCSPAsync(cert, certRoot, ocspUrls[0]);
        return result.ToString();
    }

    private static async Task<string?> ValidateCrlServerApiAsync(BigInteger serial, string apiUrl)
    {
        var configSection = PropertiesUtils.GetConfig();
        string certificadoRevocadoUrl = (string)configSection["CertificadoRevocadoUrl"];
        string revokedCertificate = string.IsNullOrEmpty(apiUrl) ? certificadoRevocadoUrl : apiUrl;
        Console.WriteLine("certificado_revocado_url " + revokedCertificate);

        if (!string.IsNullOrEmpty(revokedCertificate))
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string url = $"{revokedCertificate}/{serial}";
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
            catch (HttpRequestException ex)
            {
                await Console.Out.WriteLineAsync($"Error en la solicitud HTTP: {ex.Message}");
                throw new Exception("No se pudo Conectar a la API");
            }

        }
        else
        {
            return null;
        }

    }
}
