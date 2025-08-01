using Application.Abstractions.Security;
using Application.Responses;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        _logger.LogInformation("Registering new user with email: {Email}", dto.Email);

        if (await _unitOfWork.Users.FindByEmailAsync(dto.Email, cancellationToken) != null)
        {
            _logger.LogWarning("User with email {Email} already exists", dto.Email);
            return RegisterUserResult.Failure("User with this email already exists");
        }

        var hashedPassword = _passwordHasher.HashPassword(dto.Password);
        var user = User.CreateNew(dto.Email, hashedPassword, dto.Name, dto.LastName);

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            //todo send request in UserService

            _logger.LogInformation("Successfully registered user {Email} with ID: {UserId}", user.Email, user.Id);
            return new RegisterUserResult(user.Id, true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to register user {Email}", dto.Email);
            return RegisterUserResult.Failure("Registration failed");
        }
    }
}