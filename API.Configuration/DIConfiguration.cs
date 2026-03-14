using AppConstants;
using API.Services;
using API.Services.Abstraction;
using API.Services.Mock;
using DataAccess;
using DataAccess.Abstraction;
using Mediator;
using Mediator.Mediator.Handlers.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Models.Common;

using API.Services.Campaign;
using API.Services.Character;
using API.Services.Narration;
using API.Services.Session;
using Models.Interfaces;

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
        services.Configure<NarrationOptions>(configuration.GetSection("Narration"));
        services.Configure<LocalLlmOptions>(configuration.GetSection("LocalLlm"));

        // Register narrators with keyed HttpClient
        services.AddHttpClient(NarrationProviders.LmStudio);
        // services.AddHttpClient(NarrationProviders.Ollama);
        // services.AddHttpClient(NarrationProviders.Anthropic);

        services.AddKeyedScoped<INarrator, LocalLlmNarrator>(NarrationProviders.LmStudio, (sp, key) =>
        {
            var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient((string)key!);
            var options = sp.GetRequiredService<IOptions<LocalLlmOptions>>();
            return new LocalLlmNarrator(client, options);
        });

        // Register INarrator via NarratorFactory
        services.AddScoped<NarratorFactory>();
        services.AddScoped<INarrator>(sp => sp.GetRequiredService<NarratorFactory>().Create());

        var useMockData = configuration.GetValue<bool>("UseMockData");

        if (useMockData)
        {
            services.AddScoped<ICampaignService, MockCampaignService>();
            services.AddScoped<ICharacterService, MockCharacterService>();
        }
        else
        {
            services.AddScoped<ICampaignService, JsonCampaignService>();
            services.AddScoped<ICharacterService, JsonCharacterService>();
        }

        services.AddSingleton<ISessionStore, SessionStore>();
        services.AddScoped<ISessionPersistence, JsonSessionPersistence>();
        services.AddSingleton<IGameSystemRegistry, GameSystemRegistry>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorAssemblyMarker).Assembly));
    }
}
