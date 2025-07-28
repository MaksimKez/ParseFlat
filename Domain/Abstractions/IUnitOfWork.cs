using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{ 
    IUserRepository Users { get; }
    ITokenRepository<RefreshToken> RefreshTokens { get; }
    ITokenRepository<EmailVerificationToken> EmailVerificationTokens { get; }
    ITokenRepository<PasswordResetToken> PasswordResetTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    bool HasActiveTransaction { get; }
}