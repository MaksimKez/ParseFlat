using Application.Abstractions.JWT;
using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.LoginUserCommand;

public class LoginUserHandler(IUnitOfWork uow,
    IJwtGenerator jwtGenerator,
    ILogger<LoginUserHandler> logger,
    IPasswordHasher passwordHasher) : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        //1 exists
        //2 tokens
        //3 add db token
        //4 return

        var dto = request.Dto;
        logger.LogInformation("Login user with email {Email}", dto.Email);

        var user = await uow.Users.FindByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with email {Email} does not exist", dto.Email);
            return LoginUserResult.Failure($"User with email {dto.Email} does not exist");
        }
        
        var isValidPassword = passwordHasher.VerifyHashedPassword(user.PasswordHash, dto.Password);
        if (!isValidPassword)
        {
            logger.LogWarning("User with email {Email} does not match password", dto.Email);
            return LoginUserResult.Failure($"User with email {dto.Email} does not match password");
        }

        var refreshToken = jwtGenerator.GenerateRefreshToken(user);
        var accessToken = jwtGenerator.GenerateAccessToken(user);
        
        using var transaction = await uow.BeginTransactionAsync(cancellationToken);
        try
        {
            await uow.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            logger.LogInformation("Successfully logged in user {Email}", dto.Email);
            return LoginUserResult.Success(dto.Email, refreshToken, accessToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e, "Failed to login user {Email}", dto.Email);
            return LoginUserResult.Failure($"Failed to login user {dto.Email}");
        }
    }
}