using System.Security.Cryptography;
using Application.Common.Abstractions;

namespace Application.Common.Implementation;

public class TokenGenerator : ITokenGenerator
{
    
    public string GenerateToken(int size = 32)
    {
        var data = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }

        var base64 = Convert.ToBase64String(data);

        var base64Url = base64
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return base64Url;
    }
}