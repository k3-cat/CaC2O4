using System.Security.Cryptography;

namespace CaC2O4_Help;

class SignKey {
    internal static void Generate() {
        var cruve = ECCurve.CreateFromValue("1.2.840.10045.3.1.7");
        var ecdsa = ECDsa.Create(cruve);
        ecdsa.GenerateKey(cruve);

        Console.WriteLine("Public Key:");
        Console.WriteLine(Convert.ToBase64String(ecdsa.ExportSubjectPublicKeyInfo()));
        Console.WriteLine(String.Empty);
        Console.WriteLine("Private Key:");
        Console.WriteLine(Convert.ToBase64String(ecdsa.ExportECPrivateKey()));
    }
}
