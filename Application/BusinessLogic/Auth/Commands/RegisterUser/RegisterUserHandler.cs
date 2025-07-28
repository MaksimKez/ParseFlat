using Application.Abstractions.EmailService;
using Application.Common.Abstractions;
using Application.Responses;
using Application.Responses.Infrastructure;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.BusinessLogic.Auth.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IEmailService emailService,
        ILogger<RegisterUserHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        
        _logger.LogInformation("Registering new user with email: {Email}", dto.Email);

        try
        {
            if (await CheckUserExistsAsync(dto.Email, cancellationToken))
            {
                _logger.LogWarning("User with email {Email} already exists", dto.Email);
                return RegisterUserResponse.Failure("User with this email already exists");
            }

            var user = CreateUser(dto);
            return await RegisterUserWithEmailAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
            
            _logger.LogError(ex, "Error registering user {Email}", dto.Email);
            return RegisterUserResponse.Failure("Registration Failed");
        }
    }
    
    private User CreateUser(RegisterUserDto dto)
    {
        var id = Guid.NewGuid();
        
        return new User
        {
            Id = id,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            IsVerified = false,
            Name = dto.Name,
            LastName = dto.LastName
        };
    }

    private async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.Users.FindByEmailAsync(email);
        return existingUser != null;
    }

    private async Task<RegisterUserResponse> RegisterUserWithEmailAsync(
        User user, 
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await _unitOfWork.Users.AddAsync(user);
            
            var emailToken = _tokenGenerator.GenerateToken();
            var verificationToken = new EmailVerificationToken
            {
                Id = Guid.NewGuid(),
                Token = emailToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };
            
            await _unitOfWork.EmailVerificationTokens.AddAsync(verificationToken, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var emailResult = await _emailService.SendEmailAsync(
                user.Email, user.Name, 
                emailToken, 
                cancellationToken);

            if (!emailResult.IsSuccess)
            {
                _logger.LogError("Failed to send verification email to {Email}: {Error}", 
                    user.Email, emailResult.ErrorMessage);
                
                await transaction.RollbackAsync(cancellationToken);
                return RegisterUserResponse.Failure("Failed to send verification email");
            }
            
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully registered user {Email} with ID: {UserId}", 
                user.Email, user.Id);

            return new RegisterUserResponse(user.Id, true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to register user {Email}", user.Email);
            throw;
        }
    }
}