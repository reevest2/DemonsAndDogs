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
            NarrationOptions.LmStudio => serviceProvider.GetRequiredKeyedService<INarrator>(NarrationOptions.LmStudio),
            // Ollama: serviceProvider.GetRequiredKeyedService<INarrator>(NarrationOptions.Ollama)
            // Anthropic: serviceProvider.GetRequiredKeyedService<INarrator>(NarrationOptions.Anthropic)
            _ => throw new NotSupportedException($"Narration provider '{provider}' is not supported.")
        };
    }
}
