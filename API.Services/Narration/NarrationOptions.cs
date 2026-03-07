using AppConstants;

namespace API.Services.Narration;

public class NarrationOptions
{
    public string Provider { get; set; } = NarrationProviders.LmStudio;
}
