using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Entities;

namespace Persistence.Repositories;

public class TokenRepository<TDomain, TEntity> : ITokenRepository<TDomain>
    where TDomain : TokenBase
    where TEntity : TokenBaseEntity, new()
{
    private readonly AuthDbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger<TokenRepository<TDomain, TEntity>> _logger;

    public TokenRepository(AuthDbContext context, ILogger<TokenRepository<TDomain, TEntity>> logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task AddAsync(TDomain token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding token: {TokenId}", token.Id);
        var entity = MapToEntity(token);
        await _dbSet.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Token added: {TokenId}", token.Id);
    }

    public async Task<TDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting token by ID: {TokenId}", id);
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("Token not found by ID: {TokenId}", id);
            return null;
        }

        _logger.LogInformation("Token found by ID: {TokenId}", id);
        return MapToDomain(entity);
    }

    public async Task<TDomain?> FindByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding token by token string.");
        var entity = await _dbSet.FirstOrDefaultAsync(e => e.Token == token, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("Token not found by token string.");
            return null;
        }

        _logger.LogInformation("Token found by token string.");
        return MapToDomain(entity);
    }

    public Task RemoveAsync(TDomain token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing token: {TokenId}", token.Id);
        var entity = MapToEntity(token);
        _dbSet.Remove(entity);
        _logger.LogInformation("Token removed: {TokenId}", token.Id);
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
