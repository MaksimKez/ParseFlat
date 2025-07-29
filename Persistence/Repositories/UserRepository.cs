using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Entities;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    private readonly DbSet<UserEntity> _users;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AuthDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
        _users = _context.Set<UserEntity>();
    }

    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding user by ID: {UserId}", id);
        var entity = await _users.FindAsync([id], cancellationToken);

        if (entity == null || entity.IsDeleted)
        {
            _logger.LogWarning("User not found or deleted: {UserId}", id);
            return null;
        }

        _logger.LogInformation("User found: {UserId}", id);
        return MapToDomain(entity);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding user by email: {Email}", email);
        var entity = await _users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("User not found with email: {Email}", email);
            return null;
        }

        _logger.LogInformation("User found with email: {Email}", email);
        return MapToDomain(entity);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new user: {UserId}", user.Id);
        var entity = MapToEntity(user);
        await _users.AddAsync(entity, cancellationToken);
        _logger.LogInformation("User added to context: {UserId}", user.Id);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {UserId}", user.Id);
        var entity = await _users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("User not found for update: {UserId}", user.Id);
            return;
        }

        entity.Name = user.Name;
        entity.LastName = user.LastName;
        entity.Email = user.Email;
        entity.PasswordHash = user.PasswordHash;
        entity.IsVerified = user.IsVerified;

        _users.Update(entity);
        _logger.LogInformation("User updated: {UserId}", user.Id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting (soft) user: {UserId}", id);
        var entity = await _users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("User not found for deletion: {UserId}", id);
            return;
        }

        entity.IsDeleted = true;
        _users.Update(entity);
        _logger.LogInformation("User marked as deleted: {UserId}", id);
    }

    public async Task<IReadOnlyList<User>> ListAllAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing users: skip={Skip}, take={Take}", skip, take);
        var entities = await _users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Retrieved {Count} users", entities.Count);
        return entities.Select(MapToDomain).ToList();
    }

    private User MapToDomain(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Name = entity.Name,
            LastName = entity.LastName,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            IsVerified = entity.IsVerified
        };
    }

    private UserEntity MapToEntity(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            IsVerified = user.IsVerified,
            IsDeleted = false
        };
    }
}
