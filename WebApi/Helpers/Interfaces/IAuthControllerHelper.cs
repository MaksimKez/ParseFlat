using Microsoft.AspNetCore.Mvc;

namespace WebApi.Helpers.Interfaces;

public interface IAuthControllerHelper
{
    void SetRefreshTokenCookie(HttpResponse response, string token);
    void RemoveRefreshTokenCookie(HttpResponse response);
    bool TryGetRefreshToken(IRequestCookieCollection cookies, out string? token, out IActionResult? error, ControllerBase controller);
    bool TryGetNameFromToken(string refreshToken, out string? name, out IActionResult? error, ControllerBase controller);
}