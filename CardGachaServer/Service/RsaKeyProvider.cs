using System.Security.Cryptography;

namespace CardGachaServer.Service;

public sealed class RsaKeyProvider : IDisposable
{
    public RSA PrivateKey { get; }
    public RSA PublicKey { get; }

    public RsaKeyProvider(IConfiguration config)
    {
        var privatePath = config["PRIVATE_KEY_PATH"] ?? "/run/secrets/private_key";
        var publicPath = config["PUBLIC_KEY_PATH"] ?? "/run/secrets/public_key";
        
        PrivateKey = RSA.Create();
        PrivateKey.ImportFromPem(File.ReadAllText(privatePath));
        
        PublicKey = RSA.Create();
        PublicKey.ImportFromPem(File.ReadAllText(publicPath));
    }
    public void Dispose()
    {
        PrivateKey.Dispose();
        PublicKey.Dispose();
    }
}