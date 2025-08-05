using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmailFromToken(this ClaimsPrincipal user)
    {
        return user?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
    }
}
