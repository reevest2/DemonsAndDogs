using API.Services;
using API.Services.Abstraction;
using DataAccess;
using DataAccess.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Models.Resources;

namespace API.Configuration;

public static class DIConfiguration
{
    public static void ConfigureClients(this IServiceCollection services)
    {
        
    }
    
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IResourceRepository<JsonResource>, ResourceRepository<JsonResource>>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IJsonResourceService, JsonResourceService>();
    }
}