namespace Domain.Entities;

public abstract class TokenBase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Token { get; set; }
    
    public DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
}