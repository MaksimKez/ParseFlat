namespace Persistence.Entities;

public class TokenBaseEntity
{
    public Guid Id { get; set; }
    
    public string Token { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }

}