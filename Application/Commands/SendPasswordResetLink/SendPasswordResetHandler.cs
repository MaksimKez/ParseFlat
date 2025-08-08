using Application.Abstractions.Security;
using Application.Responses;
using MediatR;

namespace Application.Commands.SendPasswordResetLink;

public class SendPasswordResetHandler(IPasswordResetService service) : IRequestHandler<SendPasswordResetCommand, SendPasswordResetResult>
{
    public async Task<SendPasswordResetResult> Handle(SendPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        
        var result = await service.SendPasswordResetTokenAsync(email, cancellationToken);
        //todo check if success
        return result;
        
        //goal: generating token and sending link

        //1) generating token, adding (like in SendVerificationLink)
        //2) send it


        // return
        // in ResetPasswordHandler email and new password is needed
        //1) endpoint "reset-password/{token}" and {email, password} in body
        // or {email, password, token} are all in body
        // then change password, notify password changed
    }
}