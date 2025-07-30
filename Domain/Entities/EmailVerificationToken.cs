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
}