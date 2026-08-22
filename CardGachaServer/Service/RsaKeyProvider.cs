using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace CardGachaServer.Service;

public static class RsaKeyProvider
{
    private static readonly RSA _rsa = RSA.Create();

    public static RsaSecurityKey GetSecurityKey()
    {
        return new RsaSecurityKey(_rsa);
    }
}