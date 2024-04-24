namespace Singer.Domain;

public class UserCertificate
{
    public string Cedula { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Institucion { get; set; } = "";
    public string Cargo { get; set; } = "";
    public string Serial { get; set; }
    public string FechaFirmaArchivo { get; set; }
    public string EntidadCertificadora { get; set; }
    public bool SelladoTiempo { get; set; }
    public bool CertificadoDigitalValido { get; set; }

    public override string ToString()
    {
        return $"\tUser\n" +
               $"\t\t[cedula={Cedula}\n" +
               $"\t\tnombre={Nombre}\n" +
               $"\t\tapellido={Apellido}\n" +
               $"\t\tinstitucion={Institucion}\n" +
               $"\t\tcargo={Cargo}\n" +
               $"\t\tentidadCertificadora={EntidadCertificadora}\n" +
               $"\t\tselladoTiempo={SelladoTiempo}\n" +
               $"\t\tcertificadoDigitalValido={CertificadoDigitalValido}\n" +
               $"\t\t]";
    }
}
