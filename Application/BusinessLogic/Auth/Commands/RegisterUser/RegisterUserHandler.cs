using Application.Common;
using Application.Common.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
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

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.dto;

        var id = Guid.NewGuid();
        await _unitOfWork.Users.AddAsync(new User
        {
            Id = id,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            IsVerified = false,
            Name = dto.Name,
            LastName = dto.LastName
        });
        
        //generate email token
        // send conf email
        //it is pet proj, so it'd be interesting to try 
        // todo add endpoints for email conf: one for sending email and otp, second for client to send otp to server 
        return id;
    }
}