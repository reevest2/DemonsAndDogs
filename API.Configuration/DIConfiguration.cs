using API.Services;
using API.Services.Abstraction;
using DataAccess;
using DataAccess.Abstraction;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Models.Common;

namespace API.Configuration;

public static class DIConfiguration
{
    public static void ConfigureClients(this IServiceCollection services)
    {
        
    }
    
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IJsonResourceRepository, JsonResourceRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IJsonResourceService, JsonResourceService>();
        services.AddSingleton<IGameSystemRegistry, GameSystemRegistry>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorAssemblyMarker).Assembly));
    }
}
