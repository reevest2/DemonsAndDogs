using API.Services;
using API.Services.Abstraction;
using DataAccess;
using DataAccess.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Models.Character;
using Models.Resources;

namespace API.Configuration;

public static class DIConfiguration
{
    public static void ConfigureClients(this IServiceCollection services)
    {
        
    }
    
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IResourceRepository.IResourceRepository<TestResource>, ResourceRepository<TestResource>>();
        services.AddScoped<IResourceRepository.IResourceRepository<CharacterResource>, ResourceRepository<CharacterResource>>();
        services.AddScoped<IResourceRepository.IResourceRepository<CharacterTemplateData>, ResourceRepository<CharacterTemplateData>>();
        services.AddScoped<IResourceRepository.IResourceRepository<RulesetResource>, ResourceRepository<RulesetResource>>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ITestResourceService, TestResourceService>();
        services.AddScoped<ICharacterResourceService, CharacterResourceService>();
        services.AddScoped<ICharacterTemplateResourceService, CharacterTemplateResourceService>();
        services.AddScoped<IRulesetResourceService, RulesetResourceService>();
    }
}