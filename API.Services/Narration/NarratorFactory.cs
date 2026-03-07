using AppConstants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Models.Interfaces;

namespace API.Services.Narration;

public class NarratorFactory(IServiceProvider serviceProvider, IOptions<NarrationOptions> options)
{
    public INarrator Create()
    {
        var provider = options.Value.Provider;
        
        return provider switch
        {
            NarrationProviders.LmStudio => serviceProvider.GetRequiredKeyedService<INarrator>(NarrationProviders.LmStudio),
            // Ollama: serviceProvider.GetRequiredKeyedService<INarrator>(NarrationProviders.Ollama)
            // Anthropic: serviceProvider.GetRequiredKeyedService<INarrator>(NarrationProviders.Anthropic)
            _ => throw new NotSupportedException($"Narration provider '{provider}' is not supported.")
        };
    }
}
