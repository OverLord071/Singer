using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Singer.Utilities.EcuadorEntities.BancoCentral;

public abstract class CertificateBancoCentral
{
    public const string OID_CERTIFICADO_PERSONA_NATURAL = "1.3.6.1.4.1.37947.2.1.1";
    public const string OID_CERTIFICADO_PERSONA_JURIDICA = "1.3.6.1.4.1.37947.2.2.1";
    public const string OID_CERTIFICADO_FUNCIONARIO_PUBLICO = "1.3.6.1.4.1.37947.2.3.1";
    public const string OID_SELLADO_TIEMPO = "1.3.6.1.4.1.37947.4.1";

    public const string OID_CEDULA_PASAPORTE = "1.3.6.1.4.1.37947.3.1";
    public const string OID_NOMBRES = "1.3.6.1.4.1.37947.3.2";
    public const string OID_APELLIDO_1 = "1.3.6.1.4.1.37947.3.3";
    public const string OID_APELLIDO_2 = "1.3.6.1.4.1.37947.3.4";
    public const string OID_CARGO = "1.3.6.1.4.1.37947.3.5";
    public const string OID_INSTITUCION = "1.3.6.1.4.1.37947.3.6";
    public const string OID_DIRECCION = "1.3.6.1.4.1.37947.3.7";
    public const string OID_TELEFONO = "1.3.6.1.4.1.37947.3.8";
    public const string OID_CIUDAD = "1.3.6.1.4.1.37947.3.9";
    public const string OID_RAZON_SOCIAL = "1.3.6.1.4.1.37947.3.10";
    public const string OID_RUC = "1.3.6.1.4.1.37947.3.11";

    public const string OID_PAIS = "1.3.6.1.4.1.37947.3.12";

    public const string OID_CONTENEDOR = "1.3.6.1.4.1.37947.3.100";

    private readonly X509Certificate2 certificado;

    public CertificateBancoCentral(X509Certificate2 certificado)
    {
        this.certificado = certificado;
    }

    public string GetContenedor() { return GetExtension(OID_CONTENEDOR); }
    public string GetCedulaPasaporte() { return GetExtension(OID_CEDULA_PASAPORTE); }
    public string GetNombres() { return GetExtension(OID_NOMBRES); }
    public string GetPrimerApellido() { return GetExtension(OID_APELLIDO_1); }
    public string GetSegundoApellido() { return GetExtension(OID_APELLIDO_2); }
    public string GetCargo() { return GetExtension(OID_CARGO); }
    public string GetInstitucion() { return GetExtension(OID_INSTITUCION); }
    public string GetDirrecion() { return GetExtension(OID_DIRECCION); }
    public string GetTelefono() { return GetExtension(OID_TELEFONO); }
    public string GetCiudad() { return GetExtension(OID_CIUDAD); }
    public string GetPais() { return GetExtension(OID_PAIS); }
    public string GetRuc() { return GetExtension(OID_RUC); }
    public string GetRazonSocial() { return GetExtension(OID_RAZON_SOCIAL); }


    protected string GetExtension(string oid)
    {
        var extension = certificado.Extensions[oid];
        if (extension != null)
        {
            AsnEncodedData asndata = new AsnEncodedData(extension.Oid, extension.RawData);
            return asndata.Format(true);
        }
        else
        {
            return string.Empty;
        }
    }


}
