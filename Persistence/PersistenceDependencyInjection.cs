using Domain.Abstractions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connStr)
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection") ?? connStr;

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string not found in environment variables.");

        Console.WriteLine("Using connection string: " + connectionString);

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
