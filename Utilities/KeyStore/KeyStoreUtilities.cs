using Singer.Domain;
using Singer.Utilities.Certificate;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.KeyStore;

public class KeyStoreUtilities
{
    public static List<Alias> getSigningAliases(X509Store keyStore)
    {
        try
        {
            keyStore.Open(OpenFlags.ReadOnly);
            var aliasList = new List<Alias>();

            foreach (var certificate in keyStore.Certificates)
            {
                try
                {
                    certificate.Verify();
                }
                catch (CryptographicException)
                {
                    continue;
                }

                string name = CertificateUtils.GetCN(certificate.Subject);
                var keyUsage = certificate.Extensions
                    .OfType<X509KeyUsageExtension>()
                    .FirstOrDefault();

                if (keyUsage != null && keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature))
                {
                    aliasList.Add(new Alias(certificate.FriendlyName, name));
                }
            }

            return aliasList;
        }
        catch (CryptographicException)
        {
            throw new InvalidOperationException("Error al acceder al almacen de certificados");
        }
    }
}
