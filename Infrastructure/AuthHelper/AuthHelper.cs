using Application.Abstractions.AuthHelper;
using Application.Responses;

namespace Infrastructure.AuthHelper;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

public class AuthHelper : IAuthHelper
{
    public AuthHelperResult GetRefreshToken(IEnumerable<KeyValuePair<string, string>> valuePairs)
    {
        var refreshToken = valuePairs
            .FirstOrDefault(x => x.Key.Equals("refreshToken", StringComparison.OrdinalIgnoreCase))
            .Value;

        return string.IsNullOrWhiteSpace(refreshToken) 
            ? AuthHelperResult.Failure("Refresh token is missing or empty")
            : AuthHelperResult.Success(refreshToken);
    }

    public AuthHelperResult GetEmailFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return AuthHelperResult.Failure("Token is missing");

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");

            return emailClaim == null 
                ? AuthHelperResult.Failure("Email claim not found in token")
                : AuthHelperResult.Success(emailClaim.Value);
        }
        catch (Exception ex)
        {
            return AuthHelperResult.Failure($"Invalid token: {ex.Message}");
        }
    }
}
