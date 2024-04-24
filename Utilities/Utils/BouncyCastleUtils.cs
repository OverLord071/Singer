using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.Utils;

public class BouncyCastleUtils
{
    public static bool CertificateHasPolicy(X509Certificate2 cert, string oid)
    {
        foreach (var extension in cert.Extensions)
        {
            if (extension.Oid.Value == "2.5.29.32")
            {
                AsnEncodedData asnEncoded = new AsnEncodedData(extension.Oid, cert.RawData);
                string allData = asnEncoded.Format(true);

                if (allData.Contains(oid)) 
                {
                    return true;
                }
            }
        }

        return false;
    }
}
