namespace Domain.Abstractions;

public abstract class TokenBase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Token { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    
    public void MarkAsUsed()
    {
        if (IsUsed) throw new InvalidOperationException("Token is already used");
        IsUsed = true;
    }
}