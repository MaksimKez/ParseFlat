using Domain.Abstractions;

namespace Persistence;

public class AuthDbTransaction : IDbContextTransaction
{
    private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _efTransaction;

    public AuthDbTransaction(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction efTransaction)
    {
        _efTransaction = efTransaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _efTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default) => await _efTransaction.RollbackAsync(cancellationToken);

    public void Dispose() => _efTransaction.Dispose();
}
