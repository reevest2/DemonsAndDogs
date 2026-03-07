using API.Services;
using API.Services.Abstraction;
using API.Services.Mock;
using DataAccess;
using DataAccess.Abstraction;
using Mediator;
using Microsoft.Extensions.Configuration;
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

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var useMockData = configuration.GetValue<bool>("UseMockData");

        if (useMockData)
        {
            services.AddScoped<ICampaignService, MockCampaignService>();
            services.AddScoped<ICharacterService, MockCharacterService>();
        }
        else
        {
            // Real implementations will be registered here when they exist
        }

        services.AddSingleton<IGameSystemRegistry, GameSystemRegistry>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorAssemblyMarker).Assembly));
    }
}
