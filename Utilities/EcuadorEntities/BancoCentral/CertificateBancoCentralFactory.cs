using Singer.Utilities.Utils;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.EcuadorEntities.BancoCentral;

public static class CertificateBancoCentralFactory
{
    // OIDs de tipo de certificado:
    public const string OID_CERTIFICADO_PERSONA_NATURAL = "1.3.6.1.4.1.37947.2.1.1";
    public const string OID_CERTIFICADO_PERSONA_JURIDICA = "1.3.6.1.4.1.37947.2.2.1";
    public const string OID_CERTIFICADO_FUNCIONARIO_PUBLICO = "1.3.6.1.4.1.37947.2.3.1";
    public const string OID_SELLADO_TIEMPO = "1.3.6.1.4.1.37947.4.1";

    public static bool EsCertificadoDelBancoCentral(X509Certificate2 certificado)
    {
        return (BouncyCastleUtils.CertificateHasPolicy(certificado, OID_CERTIFICADO_PERSONA_NATURAL)
                || BouncyCastleUtils.CertificateHasPolicy(certificado, OID_CERTIFICADO_PERSONA_JURIDICA)
                || BouncyCastleUtils.CertificateHasPolicy(certificado, OID_CERTIFICADO_FUNCIONARIO_PUBLICO)
                || BouncyCastleUtils.CertificateHasPolicy(certificado, OID_SELLADO_TIEMPO));
    }

    //public static CertificateBancoCentral Construir(X509Certificate2 certificado)
    //{
    //    //if (BouncyCastleUtils.CertificateHasPolicy(certificado, OID_CERTIFICADO_PERSONA_NATURAL))
    //    //{
    //    //    return new CertificatePersonaNaturalBancoCentral(certificado);
    //    //}
    //    //else if (BouncyCastleUtils.CertificateHasPolicy(certificado, OID_CERTIFICADO_PERSONA_JURIDICA))
    //    //{
    //    //    return new CertificatePersonaJuridicaBancoCentral(certificado);
    //    //}
    //    //else if (BouncyCastleUtils.CertificateHasPolicy(certificado, OID_CERTIFICADO_FUNCIONARIO_PUBLICO))
    //    //{
    //    //    return new CertificateFuncionarioPublicoBancoCentral(certificado);
    //    //}
    //    //else if (BouncyCastleUtils.CertificateHasPolicy(certificado, OID_SELLADO_TIEMPO))
    //    //{
    //    //    return new CertificatePersonaNaturalBancoCentral(certificado);
    //    //}
    //    else
    //    {
    //        throw new Exception("Certificado del Banco Central del Ecuador de tipo desconocido!");
    //    }
    //}
}

