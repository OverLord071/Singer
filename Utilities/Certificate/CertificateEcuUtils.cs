using Org.BouncyCastle.Crypto.Tls;
using Singer.Domain;
using Singer.Utilities.EcuadorEntities.BancoCentral;
using Singer.Utilities.Utils;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.Certificate;

public class CertificateEcuUtils
{
    public const string UANATACA_NAME = "UANATACA S.A.";
    public const string ECLIPSOFT_NAME = "ECLIPSOFT S.A.";
    public const string DATIL_NAME = "DATILMEDIA S.A.";
    public const string AGOSDATA_NAME = "ARGOSDATA CA";
    public const string LAZZATE_NAME = "LAZZATE CIA. LTDA.";

    public static X509Certificate2 GetRootCertificate(X509Certificate2 cert)
    {
        string entidadCertStr = GetNombreCA(cert);

        return entidadCertStr switch
        {
            "Banco Central del Ecuador" => GetBancoCentralRootCertificate(cert),
            "Security Data" => GetSecurityDataRootCertificate(cert),
            //"Consejo de la Judicatura" => new X509Certificate2(/* Implementación del Certificate */),
            //"Anf AC" => GetAnfRootCertificate(cert),
            //"Dirección General de Registro Civil, Identificación y Cedulación" => new X509Certificate2(/* Implementación del Certificate */),
            UANATACA_NAME => GetUanatacaRootCertificate(cert),
            //DATIL_NAME => new X509Certificate2(/* Implementación del Certificate */),
            //AGOSDATA_NAME => new X509Certificate2(/* Implementación del Certificate */),
            //LAZZATE_NAME => new X509Certificate2(/* Implementación del Certificate */),
            _ => throw new Exception("Entidad Certificatera no reconocida")
        };
    }

    private static X509Certificate2 GetUanatacaRootCertificate(X509Certificate2 cert)
    {
        throw new NotImplementedException();
    }

    private static X509Certificate2 GetAnfRootCertificate(X509Certificate2 cert)
    {
        throw new NotImplementedException();
    }

    private static X509Certificate2 GetSecurityDataRootCertificate(X509Certificate2 cert)
    {
        throw new NotImplementedException();
    }

    private static X509Certificate2 GetBancoCentralRootCertificate(X509Certificate2 cert)
    {
        try
        {
            if (UtilsCert.VerifySignature(cert, new BceSubCaCert20112021()))
            {
                Console.WriteLine("BceSubCaCert 2011-2021");
                return new X509Certificate2(/* Implementación del Certificate */);
            }
            if (UtilsCert.VerifySignature(cert, new BceSubCaCert20192029()))
            {
                Console.WriteLine("BceSubCaCert 2019-2029");
                return new X509Certificate2(/* Implementación del Certificate */);
            }
            return null;
        }
        catch (Exception ex)
        {
            // Manejar la excepción
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    // Implementar métodos similares para las otras entidades Certificateras

    public static string GetNombreCA(X509Certificate2 cert)
    {
        string issuerName = cert.IssuerName.Name.ToUpper();

        return issuerName switch
        {
            _ when issuerName.Contains("BANCO CENTRAL DEL ECUADOR") => "Banco Central del Ecuador",
            _ when issuerName.Contains("SECURITY DATA") => "Security Data",
            _ when issuerName.Contains("CONSEJO DE LA JUDICATURA") => "Consejo de la Judicatura",
            _ when issuerName.Contains("ANF") => "Anf AC",
            _ when issuerName.Contains("DIRECCIÓN GENERAL DE REGISTRO CIVIL") => "Dirección General de Registro Civil, Identificación y Cedulación",
            _ when issuerName.Contains(UANATACA_NAME.ToUpper()) => UANATACA_NAME,
            _ when issuerName.Contains(DATIL_NAME.ToUpper()) => DATIL_NAME,
            _ when issuerName.Contains(AGOSDATA_NAME.ToUpper()) => AGOSDATA_NAME,
            _ when issuerName.Contains(LAZZATE_NAME.ToUpper()) => LAZZATE_NAME,
            _ => "Entidad no reconocida " + issuerName
        };
    }

    public static UserCertificate GetUserData(X509Certificate2 certificate)
    {
        var userData = new UserCertificate();
        userData.SelladoTiempo = false;

        if (CertificateBancoCentralFactory.EsCertificadoDelBancoCentral(certificate))
        {
            //CertificateBancoCentral CertificateBancoCentral = CertificateBancoCentralFactory.Construir(certificate);

            //userData.Cedula = CertificateBancoCentral.GetCedulaPasaporte();
            //userData.Nombre = CertificateBancoCentral.GetNombres();
            //userData.Apellido = $"{CertificateBancoCentral.GetPrimerApellido()} {CertificateBancoCentral.GetSegundoApellido()}";
            //userData.Serial = certificate.SerialNumber.ToString();

            //if (CertificateBancoCentral is CertificateFuncionarioPublico CertificateFuncionarioPublico)
            //{
            //    userData.Institucion = CertificateFuncionarioPublico.GetIntitucion();
            //    userData.Cargo = CertificateFuncionarioPublico.GetCargo();
            //}
            //else if (CertificateBancoCentral is CertificateMiembroEmpresa CertificateMiembroEmpresa)
            //{
            //    userData.Cargo = CertificateMiembroEmpresa.GetCargo();
            //}
            //else if (CertificateBancoCentral is CertificatePersonaJuridica CertificatePersonaJuridica)
            //{
            //    userData.Cargo = CertificatePersonaJuridica.GetCargo();
            //}
            //else if (CertificateBancoCentral is CertificateRepresentanteLegal CertificateRepresentanteLegal)
            //{
            //    userData.Cargo = CertificateRepresentanteLegal.GetCargo();
            //}

            //if (CertificateBancoCentral is CertificateSelladoTiempo)
            //{
            //    userData.SelladoTiempo = true;
            //}

            userData.EntidadCertificadora = "Banco Central del Ecuador";
            userData.CertificadoDigitalValido = true;
        }

        return userData;
    }
}

