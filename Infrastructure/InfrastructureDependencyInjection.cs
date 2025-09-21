using Application.Abstractions.AuthHelper;
using Application.Abstractions.EmailService;
using Application.Abstractions.JWT;
using Application.Abstractions.Security;
using Application.Abstractions.UserService;
using Application.Dtos.Settings;
using Infrastructure.Email;
using Infrastructure.JWT;
using Infrastructure.Security;
using Infrastructure.UserServiceClient.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Refit;

namespace Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<UserProfileClientSettings>()
            .BindConfiguration(UserProfileClientSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRefitClient<IUserServiceApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<UserProfileClientSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            });

        services.AddHttpClient<UserServiceClient.UserServiceClient>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<UserProfileClientSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        });

        services.AddSingleton<ResiliencePipeline>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<UserProfileClientSettings>>().Value;
            
            return new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = settings.RetryCount,
                    Delay = TimeSpan.FromSeconds(settings.RetryDelaySeconds),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = settings.UseJitter,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TaskCanceledException>()
                        .Handle<TimeoutRejectedException>()
                })
                /*.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = settings.CircuitBreakerFailureThreshold,
                    BreakDuration = TimeSpan.FromSeconds(settings.CircuitBreakerBreakDurationSeconds),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TaskCanceledException>()
                        .Handle<TimeoutRejectedException>()
                })*/
                .AddTimeout(TimeSpan.FromSeconds(settings.TimeoutSeconds))
                .Build();
        });

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
