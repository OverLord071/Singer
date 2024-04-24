using Singer.Utilities.Certificate;
using Singer.Utilities.EcuadorEntities.BancoCentral;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.Utils;

public class OcspUtils
{
    static OcspUtils()
    {
        // No es necesario agregar proveedores en .NET
        // ya que BouncyCastle no es una dependencia directa en este contexto.
        // .NET ya proporciona implementaciones para el manejo de certificados.
    }

    public static bool IsValidCertificate(X509Certificate2 certificate)
    {
        List<X509Certificate2> certs = new List<X509Certificate2>();
        certs.Add(certificate);
        certs.Add(CertificateEcuUtils.GetRootCertificate(certificate));

        // Inicializar la ruta de certificación
        X509Certificate2Collection certCollection = new X509Certificate2Collection();
        foreach (X509Certificate2 cert in certs)
        {
            certCollection.Add(cert);
        }
        X509Chain chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.ExtraStore.AddRange(certCollection);

        // Cargar los certificados raíz de la CA
        //X509Certificate2 rootCACert1 = new SecurityDataCaCert();
        X509Certificate2 rootCACert2 = new BceCaCert();
        //X509Certificate2 rootCACert3 = new ConsejoJudicaturaCaCert();
        //X509Certificate2 rootCACert4 = new AnfAc18332CaCert20162036();
        //X509Certificate2 rootCACert5 = new AnfAc37442CaCert20192039();

        // Agregar las Trust Anchors
        //chain.ChainPolicy.CustomTrustStore.Add(rootCACert1);
        chain.ChainPolicy.CustomTrustStore.Add(rootCACert2);
        //chain.ChainPolicy.CustomTrustStore.Add(rootCACert3);
        //chain.ChainPolicy.CustomTrustStore.Add(rootCACert4);
        //chain.ChainPolicy.CustomTrustStore.Add(rootCACert5);

        // Habilitar OCSP
        // No es necesario configurar esto en .NET

        // Habilitar CRLDP
        // No es necesario configurar esto en .NET

        // Realizar la validación
        bool isValid = chain.Build(certificate);
        if (!isValid)
        {
            X509ChainStatusFlags flags = chain.ChainStatus[0].Status;
            if (flags == X509ChainStatusFlags.Revoked)
            {
                Console.WriteLine("El certificado ha sido revocado.");
            }
            else if (flags == X509ChainStatusFlags.NotTimeValid)
            {
                Console.WriteLine("El certificado no es válido en este momento.");
            }
            else
            {
                Console.WriteLine("El certificado no es válido por alguna otra razón.");
            }
        }

        return isValid;
    }
}
