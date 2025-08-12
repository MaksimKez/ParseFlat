namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    
    public string PasswordHash { get; set; }

    public bool IsVerified { get; set; }
    public bool IsDeleted { get; set; }
    
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; }
    
    public static User CreateNew(
        string password,
        string name,
        string lastName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            PasswordHash = password,
            IsVerified = false,
            Name = name,
            LastName = lastName
        };
    }


}