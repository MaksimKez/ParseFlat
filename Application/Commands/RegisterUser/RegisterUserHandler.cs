using Application.Abstractions.Security;
using Application.Abstractions.UserService;
using Application.Dtos.Users;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.RegisterUser;

public class RegisterUserHandler(
    IUnitOfWork unitOfWork,
    IUserServiceClient userServiceClient,
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
        
        if ((await userServiceClient.FindByEmailAsync(request.Email, cancellationToken)).User is not null)
        {
            logger.LogWarning("User {Email} already exists", request.Email);
            return RegisterUserResult.Failure("User with this email already exists");
        }

        var hashedPassword = passwordHasher.HashPassword(request.Password);
        var user = User.CreateNew(hashedPassword, request.Name, request.LastName);

        await unitOfWork.Users.AddAsync(user, cancellationToken);
        
        var result = await userServiceClient.AddUserAsync(new UserDto()
        {
            Email = request.Email,
            Id = user.Id,
            Name = request.Name,
            LastName = request.LastName
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("Failed to add user {Email}", request.Email);
            return RegisterUserResult.Failure(result.ErrorMessage);
        }

        logger.LogInformation("Successfully registered user {Email}", request.Email);
        return RegisterUserResult.Success(user.Id);
    }
}
