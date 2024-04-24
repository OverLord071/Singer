using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using System.Security.Cryptography.X509Certificates;

namespace Singer.Utilities.OCSP;

public class ValidatorOCSP
{
    private const int TimeOut = 5000;

    public static async Task<string> ValidarOCSPAsync(X509Certificate2 checkCert, X509Certificate2 rootCert, string ocspUrl)
    {
        OcspReq request = GenerateOcspRequest(rootCert, new BigInteger(checkCert.SerialNumber));

        byte[] array = request.GetEncoded();

        using HttpClient httpClient = new();
        httpClient.Timeout = TimeSpan.FromMilliseconds(TimeOut);

        ByteArrayContent byteContent = new(array);
        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ocsp-request");

        HttpResponseMessage httpResponse = await httpClient.PostAsync(ocspUrl, byteContent);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Invalid HTTP response: {httpResponse.StatusCode}");
        }

        Stream respStream = await httpResponse.Content.ReadAsStreamAsync();
        OcspResp ocspResponse = new(respStream);

        if (ocspResponse.Status != OcspRespStatus.Successful)
        {
            throw new Exception($"Invalid OCSP response status: {ocspResponse.Status}");
        }

        BasicOcspResp basicResponse = (BasicOcspResp)ocspResponse.GetResponseObject();

        if (basicResponse == null)
        {
            throw new Exception("Invalid OCSP response");
        }

        SingleResp[] responses = basicResponse.Responses;
        SingleResp response = responses[0];
        CertificateStatus certStatus = (CertificateStatus)response.GetCertStatus();

        if (certStatus == CertificateStatus.Good)
        {
            return "Good";
        }
        else if (certStatus is RevokedStatus revokedStatus)
        {
            return $"Revoked at {revokedStatus.RevocationTime:yyyy-MM-dd HH:mm:ss}";
        }
        else
        {
            return "Unknown status";
        }
    }

    private static OcspReq GenerateOcspRequest(X509Certificate2 issuerCert, BigInteger serialNumber)
    {
        Org.BouncyCastle.X509.X509Certificate bouncyIssuerCert = DotNetUtilities.FromX509Certificate(issuerCert);

        var digestCalculator = DigestUtilities.GetDigest("SHA-1");
        var issuerNameHash = new byte[digestCalculator.GetDigestSize()];
        digestCalculator.BlockUpdate(bouncyIssuerCert.SubjectDN.GetDerEncoded(), 0, bouncyIssuerCert.SubjectDN.GetDerEncoded().Length);
        digestCalculator.DoFinal(issuerNameHash, 0);
        var id = new CertificateID(digestCalculator.ToString(), bouncyIssuerCert, serialNumber);

        OcspReqGenerator gen = new();
        gen.AddRequest(id);

        X509ExtensionsGenerator extGen = new();
        byte[] nonce = new byte[16];
        new Random().NextBytes(nonce);
        extGen.AddExtension(OcspObjectIdentifiers.PkixOcspNonce, false, new DerOctetString(nonce));
        gen.SetRequestExtensions(extGen.Generate());

        return gen.Generate();
    }
}