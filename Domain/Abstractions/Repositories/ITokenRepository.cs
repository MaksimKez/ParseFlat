namespace Domain.Abstractions.Repositories;

public interface ITokenRepository<T> where T : TokenBase
{
    Task AddAsync(T token);
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> FindByTokenAsync(string token);
    Task RemoveAsync(T token);
}
