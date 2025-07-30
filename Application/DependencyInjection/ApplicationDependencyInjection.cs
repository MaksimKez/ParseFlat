using System.Reflection;
using Application.Commands.RegisterUser;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(RegisterUserHandler).Assembly);
        });
        return services;
    }
}