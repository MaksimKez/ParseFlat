using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.JWT;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.JWT;

public class JwtGenerator : IJwtGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly SymmetricSecurityKey _securityKey;

    public JwtGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
    }

    public string GenerateAccessToken(User user)
    {
        var claims = GenerateClaims(user);
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiresMinutes);

        return GenerateToken(claims, expires);
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        var id = Guid.NewGuid();
        var claims = GenerateClaims(user, id);
        var expires = DateTime.UtcNow.AddHours(_jwtOptions.RefreshTokenExpiresHours);

        var token = GenerateToken(claims, expires);

        return new RefreshToken
        {
            Id = id,
            Token = token,
            UserId = user.Id,
            ExpiresAt = expires
        };
    }

    private Claim[] GenerateClaims(User user, Guid? tokenId = null)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, tokenId?.ToString() ?? Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        ];
    }

    private string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
    {
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int RefreshTokenExpiresHours { get; set; }
    public string SecretKey { get; set; }
    public int AccessTokenExpiresMinutes { get; set; }
}