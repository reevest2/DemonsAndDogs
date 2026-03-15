using AppConstants;
using API.Services;
using API.Services.Campaigns;
using API.Services.Characters;
using API.Services.Documents;
using API.Services.GameSystems;
using API.Services.Mock;
using API.Services.Narration;
using API.Services.Sessions;
using DataAccess;
using DataAccess.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Common;
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
            var logger = sp.GetRequiredService<ILogger<LocalLlmNarrator>>();
            return new LocalLlmNarrator(client, options, logger);
        });

        // Register INarrator via NarratorFactory
        services.AddScoped<NarratorFactory>();
        services.AddScoped<INarrator>(sp =>
        {
            var result = sp.GetRequiredService<NarratorFactory>().Create();
            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to create narrator: {result.Error!.Message}");
            return result.Value!;
        });

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
            services.AddScoped<IDocumentService, JsonDocumentService>();
        }

        services.AddSingleton<ISessionStore, SessionStore>();
        services.AddScoped<ISessionPersistence, JsonSessionPersistence>();
        services.AddSingleton<IGameSystemRegistry, GameSystemRegistry>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SessionStore).Assembly));
    }
}
