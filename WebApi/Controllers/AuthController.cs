using System.IdentityModel.Tokens.Jwt;
using Application.Abstractions.AuthHelper;
using Application.Commands.LoginUserCommand;
using Application.Commands.RefreshAccessToken;
using Application.Commands.RegisterUser;
using Application.Commands.SendVerificationLink;
using Application.Commands.VerifyEmail;
using Application.Dtos.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, IAuthHelper authHelper) : ControllerBase
{
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

        // Store refresh token in cookie
        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        });

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

        var result = await mediator.Send(new RefreshAccessTokenCommand(refreshTokenResult.Value));

        return result.IsSuccess
            ? Ok(new { accessToken = result.Token })
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost("sendverificationlink")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendVerificationLink()
    {
        var refreshTokenResult = authHelper.GetRefreshToken(Request.Cookies);

        if (!refreshTokenResult.IsSuccess || refreshTokenResult.Value == null)
            return BadRequest(refreshTokenResult.ErrorMessage);

        var emailResult = authHelper.GetEmailFromToken(refreshTokenResult.Value);

        if (!emailResult.IsSuccess)
            return BadRequest(emailResult.ErrorMessage);

        var result = await mediator.Send(new SendVerificationLinkCommand(emailResult.Value!));

        if (result.IsSuccess)
        {
            Response.Cookies.Delete("refreshToken");
            return Ok("Verification link sent");
        }

        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("verifyemail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Verification token is missing");

        var result = await mediator.Send(new VerifyEmailCommand(token));

        return result.IsSuccess
            ? Ok("Email successfully verified")
            : BadRequest(result.ErrorMessage);
    }
}
