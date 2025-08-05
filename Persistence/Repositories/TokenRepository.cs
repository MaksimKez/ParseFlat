using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories;

public class TokenRepository<TDomain> : ITokenRepository<TDomain>
    where TDomain : TokenBase
{
    private readonly AuthDbContext _context;
    private readonly DbSet<TDomain> _dbSet;
    private readonly ILogger<TokenRepository<TDomain>> _logger;

    public TokenRepository(AuthDbContext context, ILogger<TokenRepository<TDomain>> logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = _context.Set<TDomain>();
    }

    public async Task AddAsync(TDomain token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding token: {TokenId}", token.Id);
        await _dbSet.AddAsync(token, cancellationToken);
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
        return entity;
    }

    public async Task<TDomain?> FindByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding token by token string.");
        var entity = await _dbSet
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Token == token, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("Token not found by token string.");
            return null;
        }

        _logger.LogInformation("Token found by token string.");
        return entity;
    }

    public void Update(TDomain token, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(token);
    }

    public Task RemoveAsync(TDomain token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing token: {TokenId}", token.Id);
        _dbSet.Remove(token);
        _logger.LogInformation("Token removed: {TokenId}", token.Id);
        return Task.CompletedTask;
    }
}
