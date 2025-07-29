namespace Persistence.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public bool IsVerified { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; }
    public ICollection<PasswordResetTokenEntity> PasswordResetTokens { get; set; }
    public ICollection<EmailVerificationTokenEntity> EmailVerificationTokens { get; set; }
}