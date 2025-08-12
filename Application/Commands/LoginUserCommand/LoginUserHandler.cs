using Application.Abstractions.JWT;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.LoginUserCommand;

public class LoginUserHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtGenerator jwtGenerator,
    ILogger<LoginUserHandler> logger)
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(
        LoginUserCommand command, 
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        logger.LogInformation("Logging in user {Email}", request.Name);

        var user = await unitOfWork.Users.FindByNameAsync(request.Name, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {Email} not found", request.Name);
            return LoginUserResult.Failure($"User with email {request.Name} does not exist");
        }

        var isValid = passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password);
        if (!isValid)
        {
            logger.LogWarning("Invalid password for user {Name}", request.Name);
            return LoginUserResult.Failure($"User with name {request.Name} does not match password");
        }

        var refreshToken = jwtGenerator.GenerateRefreshToken(user);
        var accessToken = jwtGenerator.GenerateAccessToken(user);

        await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        logger.LogInformation("User {Email} logged in", user.Name);
        return LoginUserResult.Success(user.Name, refreshToken.Token, accessToken);
    }
}
