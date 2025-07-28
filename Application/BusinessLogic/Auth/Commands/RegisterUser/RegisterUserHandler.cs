using Domain.Abstractions;
using MediatR;

namespace Application.BusinessLogic.Auth.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    

    public RegisterUserHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        /*
         * 1) hash password
         * 2) save user
         * 3) generate email token
         * 4) send conf email
         * 5) return generated ID
         */
        return null;
    }
}