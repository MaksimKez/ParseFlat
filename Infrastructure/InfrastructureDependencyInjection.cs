using Application.Abstractions.EmailService;
using Application.Abstractions.JWT;
using Application.Abstractions.Security;
using Infrastructure.Email;
using Infrastructure.Email.Interfaces;
using Infrastructure.JWT;
using Microsoft.Extensions.Configuration;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddTransient<IEmailBuilder, EmailBuilder>();
        services.AddScoped<IEmailSender, EmailSender>();
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
            
        return services;
    }
}