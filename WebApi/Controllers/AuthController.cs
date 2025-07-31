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
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await mediator.Send(new RegisterUserCommand(registerUserRequest));
        
        return result.IsSuccess
            ? Ok(new { message = "User registration successful." })
            : BadRequest(new { error = result.ErrorMessage });

    }
}