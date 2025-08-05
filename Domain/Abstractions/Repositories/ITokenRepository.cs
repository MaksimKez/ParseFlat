using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface ITokenRepository<T> where T : TokenBase
{
    Task AddAsync(T token, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> FindByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RemoveAsync(T token, CancellationToken cancellationToken = default);
}
