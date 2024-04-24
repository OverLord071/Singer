using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Singer.Utilities.Certificate;

namespace Singer.Utilities.Utils;

public class UtilsCert
{
    public static bool VerifySignature(X509Certificate2 certificate)
    {
        try
        {
            X509Certificate2 rootCertificate = CertificateEcuUtils.GetRootCertificate(certificate);
            if (rootCertificate != null)
            {
                using (RSA publicKeyForSignature = rootCertificate.GetRSAPublicKey())
                {
                    byte[] signature = certificate.GetCertHash();
                    byte[] dataToVerify = certificate.RawData;

                    return publicKeyForSignature.VerifyData(dataToVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Signature verification failed: {ex.Message}");
            return false;
        }
    }

    public static bool VerifySignature(X509Certificate2 certificate, X509Certificate2 rootCertificate)
    {
        try
        {
            if (rootCertificate != null)
            {
                using (RSA publicKeyForSignature = rootCertificate.GetRSAPublicKey())
                {
                    byte[] signature = certificate.GetCertHash();
                    byte[] dataToVerify = certificate.RawData;

                    return publicKeyForSignature.VerifyData(dataToVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Signature verification failed: {ex.Message}");
            return false;
        }
    }
}
