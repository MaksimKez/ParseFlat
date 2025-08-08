using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public IUserRepository Users { get; }
    public ITokenRepository<RefreshToken> RefreshTokens { get; }
    public ITokenRepository<EmailVerificationToken> EmailVerificationTokens { get; }
    public ITokenRepository<PasswordResetToken> PasswordResetTokens { get; }

    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    public UnitOfWork(
        AuthDbContext context,
        IUserRepository users,
        ITokenRepository<RefreshToken> refreshTokens,
        ITokenRepository<EmailVerificationToken> emailVerificationTokens,
        ITokenRepository<PasswordResetToken> passwordResetTokens,
        ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
        Users = users;
        RefreshTokens = refreshTokens;
        EmailVerificationTokens = emailVerificationTokens;
        PasswordResetTokens = passwordResetTokens;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving changes to the database.");
        var result = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Changes saved: {Count} entries affected.", result);
        return result;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (HasActiveTransaction)
        {
            _logger.LogWarning("Attempted to begin a new transaction while another is active.");
            return new AuthDbTransaction(_currentTransaction!);
        }

        _logger.LogInformation("Beginning new transaction.");
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogInformation("Transaction started.");
        return new AuthDbTransaction(_currentTransaction);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!HasActiveTransaction)
        {
            _logger.LogError("Attempted to commit with no active transaction.");
            throw new InvalidOperationException("No active transaction");
        }

        _logger.LogInformation("Committing transaction.");
        await _currentTransaction!.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
        _logger.LogInformation("Transaction committed.");
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!HasActiveTransaction)
        {
            _logger.LogError("Attempted to rollback with no active transaction.");
            throw new InvalidOperationException("No active transaction");
        }

        _logger.LogWarning("Rolling back transaction.");
        await _currentTransaction!.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
        _logger.LogInformation("Transaction rolled back.");
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing UnitOfWork.");
        _context.Dispose();
    }
}
