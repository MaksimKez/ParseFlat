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
        
        logger.LogInformation("Logging in user {Email}", request.Email);

        var user = await unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {Email} not found", request.Email);
            return LoginUserResult.Failure($"User with email {request.Email} does not exist");
        }

        var isValid = passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password);
        if (!isValid)
        {
            logger.LogWarning("Invalid password for user {Email}", request.Email);
            return LoginUserResult.Failure($"User with email {request.Email} does not match password");
        }

        // Генерируем токены
        var refreshToken = jwtGenerator.GenerateRefreshToken(user);
        var accessToken = jwtGenerator.GenerateAccessToken(user);

        await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        logger.LogInformation("User {Email} logged in", user.Email);
        return LoginUserResult.Success(user.Email, refreshToken, accessToken);
    }
}
