using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    private readonly DbSet<UserEntity> _users;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
        _users = _context.Set<UserEntity>();
    }

    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _users.FindAsync([id], cancellationToken);

        return entity == null || entity.IsDeleted ? null : MapToDomain(entity);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var entity = await _users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

        return entity == null ? null : MapToDomain(entity);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(user);
        await _users.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = await _users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken: cancellationToken);
        if (entity == null) return;

        entity.Name = user.Name;
        entity.LastName = user.LastName;
        entity.Email = user.Email;
        entity.PasswordHash = user.PasswordHash;
        entity.IsVerified = user.IsVerified;

        _users.Update(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken: cancellationToken);
        if (entity == null) return;

        entity.IsDeleted = true;
        _users.Update(entity);
    }

    public async Task<IReadOnlyList<User>> ListAllAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        var entities = await _users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken: cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }


    //todo automapper
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