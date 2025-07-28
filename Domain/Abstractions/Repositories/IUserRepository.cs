using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(Guid id);

    Task<User?> FindByEmailAsync(string email);

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task DeleteAsync(Guid id);

    Task<IReadOnlyList<User>> ListAllAsync();
}