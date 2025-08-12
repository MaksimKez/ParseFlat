using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    private readonly DbSet<User> _users;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AuthDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
        _users = _context.Set<User>();
    }

    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding user by ID: {UserId}", id);
        var user = await _users.FindAsync([id], cancellationToken);

        if (user == null || user.IsDeleted)
        {
            _logger.LogWarning("User not found or deleted: {UserId}", id);
            return null;
        }

        _logger.LogInformation("User found: {UserId}", id);
        return user;
    }

    public async Task<User?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding user by email: {Email}", name);
        var user = await _users
                            .FirstOrDefaultAsync(u => u.Name == name && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found with email: {Email}", name);
            return null;
        }

        _logger.LogInformation("User found with email: {Email}", name);
        return user;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new user: {UserId}", user.Id);
        await _users.AddAsync(user, cancellationToken);
        _logger.LogInformation("User added to context: {UserId}", user.Id);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {UserId}", user.Id);
        var existingUser = await _users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (existingUser == null)
        {
            _logger.LogWarning("User not found for update: {UserId}", user.Id);
            return;
        }

        existingUser.Name = user.Name;
        existingUser.LastName = user.LastName;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.IsVerified = user.IsVerified;

        _users.Update(existingUser);
        _logger.LogInformation("User updated: {UserId}", user.Id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting (soft) user: {UserId}", id);
        var user = await _users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for deletion: {UserId}", id);
            return;
        }

        user.IsDeleted = true;
        _users.Update(user);
        _logger.LogInformation("User marked as deleted: {UserId}", id);
    }

    public async Task<IReadOnlyList<User>> ListAllAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing users: skip={Skip}, take={Take}", skip, take);
        var users = await _users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} users", users.Count);
        return users;
    }
}
