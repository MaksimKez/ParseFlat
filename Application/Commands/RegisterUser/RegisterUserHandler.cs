using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.RegisterUser;

public class RegisterUserHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ILogger<RegisterUserHandler> logger)
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand command, 
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        logger.LogInformation("Registering user {Email}", request.Email);
        
        if (await unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken) is not null)
        {
            logger.LogWarning("User {Email} already exists", request.Email);
            return RegisterUserResult.Failure("User with this email already exists");
        }

        var hashedPassword = passwordHasher.HashPassword(request.Password);
        var user = User.CreateNew(request.Email, hashedPassword, request.Name, request.LastName);

        await unitOfWork.Users.AddAsync(user, cancellationToken);

        logger.LogInformation("Successfully registered user {Email}", user.Email);
        return RegisterUserResult.Success(user.Id);
    }
}
