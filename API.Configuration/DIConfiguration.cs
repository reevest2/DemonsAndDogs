using Microsoft.Extensions.DependencyInjection;
using Models;
using ResourceFramework.Server.DataAccess;
using ResourceFramework.Server.Services;

namespace API.Configuration;

public static class DIConfiguration
{
    public static void ConfigureClients(this IServiceCollection services)
    {
        
    }

    public static void ConfigureResources(this IServiceCollection services)
    {
        services.AddScoped(typeof(IResourceService<>), typeof(ResourceService<>));

        services.AddResources(registry =>
        {
            registry.AddResource<TestResource>(AppConstants.ResourceTableNames.TestResources);
        });
    }
}
