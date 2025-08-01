using Application.Common.Interfaces;
using Application.Responses;
using MediatR;

namespace Application.Commands.LoginUserCommand;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUserService _userService;

    public LoginUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        => _userService.LoginAsync(request.Dto, cancellationToken);
}
