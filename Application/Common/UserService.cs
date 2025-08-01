using Application.Abstractions.JWT;
using Application.Abstractions.Security;
using Application.Common.Interfaces;
using Application.Dtos.Users;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Common;

public class UserService(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtGenerator jwtGenerator,
    ILogger<UserService> logger)
    : IUserService
{
    public async Task<RegisterUserResult> RegisterAsync(RegisterUserRequest dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering user {Email}", dto.Email);

        if (await unitOfWork.Users.FindByEmailAsync(dto.Email, cancellationToken) is not null)
        {
            logger.LogWarning("User {Email} already exists", dto.Email);
            return RegisterUserResult.Failure("User with this email already exists");
        }

        var hashedPassword = passwordHasher.HashPassword(dto.Password);
        var user = User.CreateNew(dto.Email, hashedPassword, dto.Name, dto.LastName);

        using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await unitOfWork.Users.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Successfully registered user {Email}", user.Email);
            return new RegisterUserResult(user.Id, true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to register user {Email}", dto.Email);
            return RegisterUserResult.Failure("Registration failed");
        }
    }

    public async Task<LoginUserResult> LoginAsync(LoginUserRequest dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Logging in user {Email}", dto.Email);

        var user = await unitOfWork.Users.FindByEmailAsync(dto.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {Email} not found", dto.Email);
            return LoginUserResult.Failure($"User with email {dto.Email} does not exist");
        }

        var isValid = passwordHasher.VerifyHashedPassword(user.PasswordHash, dto.Password);
        if (!isValid)
        {
            logger.LogWarning("Invalid password for user {Email}", dto.Email);
            return LoginUserResult.Failure($"User with email {dto.Email} does not match password");
        }

        var refreshToken = jwtGenerator.GenerateRefreshToken(user);
        var accessToken = jwtGenerator.GenerateAccessToken(user);

        using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("User {Email} logged in", user.Email);
            return LoginUserResult.Success(user.Email, refreshToken, accessToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Failed to login user {Email}", dto.Email);
            return LoginUserResult.Failure($"Failed to login user {dto.Email}");
        }
    }
}
