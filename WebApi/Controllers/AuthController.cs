using Application.Abstractions.AuthHelper;
using Application.Commands.LoginUserCommand;
using Application.Commands.RefreshAccessToken;
using Application.Commands.RegisterUser;
using Application.Commands.ResetPassword;
using Application.Commands.SendVerificationLink;
using Application.Commands.VerifyEmail;
using Application.Dtos;
using Application.Dtos.Settings;
using Application.Dtos.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, IAuthHelper authHelper, IOptions<AuthOptions> options) : ControllerBase
{
    private readonly AuthOptions authOptions = options.Value;
    
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await mediator.Send(new RegisterUserCommand(registerUserRequest));

        return result.IsSuccess
            ? CreatedAtAction(nameof(RegisterUser), new { id = result.RegisteredUserId }, null)
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
    {
        var result = await mediator.Send(new LoginUserCommand(loginUserRequest));

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshTokenResult = authHelper.GetRefreshToken(Request.Cookies);

        if (!refreshTokenResult.IsSuccess)
            return BadRequest(refreshTokenResult.ErrorMessage);

        var result = await mediator.Send(new RefreshAccessTokenCommand(refreshTokenResult.Value!));

        return result.IsSuccess
            ? Ok(new { accessToken = result.Token })
            : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpPost("sendverificationlink")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendVerificationLink(bool isEmailVerification)
    {
        var refreshTokenResult = authHelper.GetRefreshToken(Request.Cookies);

        if (!refreshTokenResult.IsSuccess || refreshTokenResult.Value == null)
            return BadRequest(refreshTokenResult.ErrorMessage);

        var nameResult = authHelper.GetNameFromToken(refreshTokenResult.Value);

        if (!nameResult.IsSuccess)
            return BadRequest(nameResult.ErrorMessage);

        var result = await mediator.Send(new SendVerificationLinkCommand(nameResult.Value!, isEmailVerification));

        if (result.IsSuccess)
        {
            Response.Cookies.Delete(authOptions.Cookie.RefreshTokenName);
            return Ok(authOptions.Messages.VerificationLinkSent);
        }

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("verifyemail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(authOptions.Messages.VerificationTokenMissing);

        var result = await mediator.Send(new VerifyEmailCommand(token));

        return result.IsSuccess
            ? Ok(authOptions.Messages.EmailVerified)
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost("changepassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var result = await mediator.Send(new ResetPasswordCommand(resetPasswordRequest));
        
        return result.IsSuccess
            ? Ok(authOptions.Messages.PasswordChanged)
            : BadRequest(result.ErrorMessage);
    }

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append(authOptions.Cookie.RefreshTokenName, token, new CookieOptions
        {
            HttpOnly = authOptions.Cookie.HttpOnly,
            Secure = authOptions.Cookie.Secure,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(authOptions.Cookie.ExpirationHours)
        });
    }
}

