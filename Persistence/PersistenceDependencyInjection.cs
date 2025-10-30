using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                               ?? Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string not found");
        }
        
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString));

        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenRepository<RefreshToken>, TokenRepository<RefreshToken>>();
        services.AddScoped<ITokenRepository<EmailVerificationToken>, TokenRepository<EmailVerificationToken>>();
        services.AddScoped<ITokenRepository<PasswordResetToken>, TokenRepository<PasswordResetToken>>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

            
        return services;
    }

}
