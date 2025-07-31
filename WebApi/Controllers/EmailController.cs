using Application.Commands.SendVerificationEmail;
using Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class EmailController(IMediator mediator) : ControllerBase
{
    [HttpPost("SendVerificationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendVerificationEmail([FromBody] SendVerificationEmailRequest request)
    {//fluentvalidation ll be added later 
        var result = await mediator.Send(new SendVerificationEmailCommand(request));

        return result.IsSuccess
            ? Ok(new { message = "Verification email sent successfully." })
            : BadRequest(new { error = result.ErrorMessage });
    }
}