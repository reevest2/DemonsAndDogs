using AppConstants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Models;
using Models.Interfaces;

namespace API.Services.Narration;

public class NarratorFactory(IServiceProvider serviceProvider, IOptions<NarrationOptions> options)
{
    public Result<INarrator> Create()
    {
        var provider = options.Value.Provider;

        return provider switch
        {
            NarrationProviders.LmStudio => Result<INarrator>.Ok(
                serviceProvider.GetRequiredKeyedService<INarrator>(NarrationProviders.LmStudio)),
            _ => Result<INarrator>.Unsupported($"Narration provider '{provider}' is not supported.")
        };
    }
}
