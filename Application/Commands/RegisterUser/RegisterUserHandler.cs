using Application.Common.Interfaces;
using Application.Responses;
using MediatR;

namespace Application.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserService _userService;

    public RegisterUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return _userService.RegisterAsync(request.Dto, cancellationToken);
    }
}
