using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        // Create directories
        Directory.CreateDirectory("ca");
        Directory.CreateDirectory("server");
        Directory.CreateDirectory("client");

        // Generate CA certificate
        var ca = GenerateCertificate("Calculator CA", true);
        File.WriteAllBytes("ca/ca.pfx", ca.Export(X509ContentType.Pfx));

        // Generate Server certificate
        var server = GenerateCertificate("calculator-server", false, ca);
        File.WriteAllBytes("server/server.pfx", server.Export(X509ContentType.Pfx));

        // Generate Client certificate
        var client = GenerateCertificate("calculator-client", false, ca);
        File.WriteAllBytes("client/client.pfx", client.Export(X509ContentType.Pfx));

        Console.WriteLine("Certificates generated successfully!");
    }

    private static X509Certificate2 GenerateCertificate(string subjectName, bool isCA, X509Certificate2? issuer = null)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            $"CN={subjectName}",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        if (isCA)
        {
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, true, 12, true));
        }

        var certificate = isCA
            ? request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10))
            : request.Create(issuer!, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1), Guid.NewGuid().ToByteArray());

        return new X509Certificate2(certificate.Export(X509ContentType.Pfx), (string?)null,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
    }
}
