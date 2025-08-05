using Application.Commands.LoginUserCommand;
using Application.Commands.RefreshAccessToken;
using Application.Commands.RegisterUser;
using Application.Commands.SendVerificationLink;
using Application.Dtos.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await mediator.Send(new RegisterUserCommand(registerUserRequest));
        
        return result.IsSuccess
            ? CreatedAtAction(nameof(RegisterUser), result.RegisteredUserId)
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        var result = await mediator.Send(new LoginUserCommand(loginUserRequest));

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

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
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest("Refresh token is missing");

        var result = await mediator.Send(new RefreshAccessTokenCommand(refreshToken));
        
        return result.IsSuccess
            ? Ok(new { accessToken = result.Token })
            : BadRequest(result.ErrorMessage);
    }

    [HttpPost("sendverificationlink")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendVerificationLink()
    {
        var email = HttpContext.User.GetEmailFromToken();
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email not found in token");

        var result = await mediator.Send(new SendVerificationLinkCommand(email));
        
        return result.IsSuccess
            ? Ok("Verification link sent")
            : BadRequest(result.ErrorMessage);
    }

    
    [HttpPost("verifyemail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Verification token is missing");

        //var result = await mediator.Send(new VerifyEmailCommand(token));

        //return result.IsSuccess
        //    ? Ok("Email successfully verified")
        //    : BadRequest(result.ErrorMessage);
        return Ok();
    }

}
