using Application.Abstractions.AuthHelper;
using Application.Dtos.Settings;
using Application.Responses;
using Microsoft.Extensions.Options;

namespace Infrastructure.AuthHelper;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

public class AuthHelper(IOptions<AuthOptions> authOptions) : IAuthHelper
{
    private readonly AuthOptions authOptions = authOptions.Value;
    public AuthHelperResult GetRefreshToken(IEnumerable<KeyValuePair<string, string>> valuePairs)
    {
        var refreshToken = valuePairs
            .FirstOrDefault(x => x.Key.Equals(authOptions.Cookie.RefreshTokenName,
                StringComparison.OrdinalIgnoreCase))
            .Value;

        return string.IsNullOrWhiteSpace(refreshToken) 
            ? AuthHelperResult.Failure(authOptions.Messages.RefreshTokenNotFound)
            : AuthHelperResult.Success(refreshToken);
    }

    public AuthHelperResult GetEmailFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return AuthHelperResult.Failure(authOptions.Messages.RefreshTokenMissing);

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == authOptions.Jwt.EmailClaimType);

            return emailClaim == null 
                ? AuthHelperResult.Failure(authOptions.Messages.EmailNotFoundInToken)
                : AuthHelperResult.Success(emailClaim.Value);
        }
        catch (Exception ex)
        {
            return AuthHelperResult.Failure(authOptions.Messages.InvalidTokenFormat);
        }
    }
}
