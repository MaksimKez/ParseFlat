using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> FindByNameAsync(string email, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListAllAsync(int skip, int take, CancellationToken cancellationToken = default);
}