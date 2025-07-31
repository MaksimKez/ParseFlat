using Domain.Abstractions;

namespace Domain.Entities;

public class EmailVerificationToken : TokenBase
{
    public bool IsUsed { get; set; }

    public void MarkAsUsed()
    {
        if (IsUsed) throw new InvalidOperationException("Token is already used");
        IsUsed = true;
    }

    public static EmailVerificationToken CreateNew(string token, Guid userId)
    {
        return new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Token = token,
            IsUsed = false,
            UserId = userId
        };
    }
}