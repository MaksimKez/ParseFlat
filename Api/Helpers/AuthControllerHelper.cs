using Api.Helpers.Interfaces;
using Application.Abstractions.AuthHelper;
using Application.Dtos.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Helpers;

public class AuthControllerHelper(IAuthHelper authHelper, IOptions<AuthOptions> options) : IAuthControllerHelper
{
    private readonly AuthOptions authOptions = options.Value;
    public bool TryGetRefreshToken(IRequestCookieCollection cookies, out string? token, out IActionResult? error, ControllerBase controller)
    {
        var res = authHelper.GetRefreshToken(cookies);
        if (!res.IsSuccess || string.IsNullOrEmpty(res.Value))
        {
            token = null;
            error = controller.BadRequest(res.ErrorMessage);
            return false;
        }
        token = res.Value;
        error = null;
        return true;
    }

    public bool TryGetNameFromToken(string refreshToken, out string? name, out IActionResult? error, ControllerBase controller)
    {
        var res = authHelper.GetNameFromToken(refreshToken);
        if (!res.IsSuccess || string.IsNullOrEmpty(res.Value))
        {
            name = null;
            error = controller.BadRequest(res.ErrorMessage);
            return false;
        }
        name = res.Value;
        error = null;
        return true;
    }

    public void SetRefreshTokenCookie(HttpResponse response, string token)
    {
        response.Cookies.Append(authOptions.Cookie.RefreshTokenName, token, new CookieOptions
        {
            HttpOnly = authOptions.Cookie.HttpOnly,
            Secure = authOptions.Cookie.Secure,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(authOptions.Cookie.ExpirationHours),
            IsEssential = true
        });
    }

    public void RemoveRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete(authOptions.Cookie.RefreshTokenName);
    }
}
