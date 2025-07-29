using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Repositories;

public class TokenRepository<TDomain, TEntity> : ITokenRepository<TDomain>
    where TDomain : TokenBase
    where TEntity : TokenBaseEntity, new()
{
    private readonly AuthDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public TokenRepository(AuthDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task AddAsync(TDomain token, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(token);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<TDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        return entity == null ? null : MapToDomain(entity);
    }

    public async Task<TDomain?> FindByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(e => e.Token == token, cancellationToken);
        return entity == null ? null : MapToDomain(entity);
    }

    public Task RemoveAsync(TDomain token, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(token);
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    // todo automapper
    protected virtual TDomain MapToDomain(TEntity entity)
    {
        throw new NotImplementedException("Provide mapping in derived class");
    }

    protected virtual TEntity MapToEntity(TDomain domain)
    {
        throw new NotImplementedException("Provide mapping in derived class");
    }
}
