using Application.Commands.LoginUserCommand;
using Application.Commands.RegisterUser;
using Application.Dtos.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        var result = await mediator.Send(new LoginUserCommand(loginUserRequest));

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new { accessToken = result.AccessToken });

    }
}
