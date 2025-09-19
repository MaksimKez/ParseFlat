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
using WebApi.Helpers.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IMediator mediator,
    IOptions<AuthOptions> options,
    ILogger<AuthController> logger,
    IAuthControllerHelper helper)
    : ControllerBase
{
    private readonly AuthOptions _authOptions = options.Value;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest registerUserRequest, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RegisterUserCommand(registerUserRequest), cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("User registered successfully with Id {UserId}", result.RegisteredUserId);
            return CreatedAtAction(nameof(RegisterUser), new { id = result.RegisteredUserId }, null);
        }

        logger.LogWarning("User registration failed: {Error}", result.ErrorMessage);
        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginUserCommand(loginUserRequest), cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogWarning("Login failed for {Name}: {Error}", loginUserRequest.Name, result.ErrorMessage);
            return BadRequest(result.ErrorMessage);
        }

        helper.SetRefreshTokenCookie(Response, result.RefreshToken);
        logger.LogInformation("User {Name} logged in successfully", loginUserRequest.Name);

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        if (!helper.TryGetRefreshToken(Request.Cookies, out var refreshToken, out var error, this))
            return error!;

        var result = await mediator.Send(new RefreshAccessTokenCommand(refreshToken!), cancellationToken);

        return result.IsSuccess
            ? Ok(new { accessToken = result.Token })
            : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpPost("sendverificationlink")]
    public async Task<IActionResult> SendVerificationLink(bool isEmailVerification, CancellationToken cancellationToken)
    {
        if (!helper.TryGetRefreshToken(Request.Cookies, out var refreshToken, out var error, this))
            return error!;

        if (!helper.TryGetNameFromToken(refreshToken!, out var name, out error, this))
            return error!;

        var result = await mediator.Send(new SendVerificationLinkCommand(name!, isEmailVerification), cancellationToken);

        if (result.IsSuccess)
        {
            helper.RemoveRefreshTokenCookie(Response);
            return Ok(_authOptions.Messages.VerificationLinkSent);
        }

        logger.LogWarning("Send verification link failed: {Error}", result.ErrorMessage);
        return BadRequest(result.ErrorMessage);
    }

    [HttpPost("verifyemail")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(_authOptions.Messages.VerificationTokenMissing);

        var result = await mediator.Send(new VerifyEmailCommand(token), cancellationToken);

        return result.IsSuccess
            ? Ok(_authOptions.Messages.EmailVerified)
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new PasswordResetCommand(resetPasswordRequest), cancellationToken);

        return result.IsSuccess
            ? Ok(_authOptions.Messages.PasswordChanged)
            : BadRequest(result.ErrorMessage);
    }
}
