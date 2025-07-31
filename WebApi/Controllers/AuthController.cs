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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest registerUserRequest)
    {
        var result = await mediator.Send(new RegisterUserCommand(registerUserRequest));
        
        return result.IsSuccess
                                //todo change to get when added
            ? CreatedAtAction("RegisterUser", new {id = result.RegisteredUserId})
            : BadRequest(new { error = result.ErrorMessage });

    }
}