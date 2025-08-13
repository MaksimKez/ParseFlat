using Application.Abstractions.AuthHelper;
using Application.Abstractions.EmailService;
using Application.Abstractions.JWT;
using Application.Abstractions.Security;
using Application.Abstractions.UserService;
using Infrastructure.Email;
using Infrastructure.JWT;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<IUserServiceClient, UserServiceClient.UserServiceClient>();

        services.AddScoped<IUserServiceClient, UserServiceClient.UserServiceClient>();
        services.AddScoped<IAuthHelper, AuthHelper.AuthHelper>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailVerifier, EmailVerifier>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        return services;
    }
}