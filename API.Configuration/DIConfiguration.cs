using API.Services;
using API.Services.Abstraction;
using DataAccess;
using DataAccess.Abstraction;
using Microsoft.Extensions.DependencyInjection;

using Models.Resources.Ruleset;

namespace API.Configuration;

public static class DIConfiguration
{
    public static void ConfigureClients(this IServiceCollection services)
    {
        
    }
    
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IResourceRepository<RulesetData>, ResourceRepository<RulesetData>>();
        services.AddScoped<IResourceRepository<CampaignData>, ResourceRepository<CampaignData>>();
        services.AddScoped<IResourceRepository<EntityData>, ResourceRepository<EntityData>>();
        services.AddScoped<IResourceRepository<TemplateData>, ResourceRepository<TemplateData>>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IRulesetResourceService, RulesetResourceService>();
        services.AddScoped<ITemplateResourceService, TemplateResourceService>();
        services.AddScoped<IEntityResourceService, EntityResourceService>();
        services.AddScoped<ICampaignResourceService, CampaignResourceService>();
    }
}