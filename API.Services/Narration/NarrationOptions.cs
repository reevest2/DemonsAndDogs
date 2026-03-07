namespace API.Services.Narration;

public class NarrationOptions
{
    public const string LmStudio = "LmStudio";
    public const string Ollama = "Ollama";
    public const string Anthropic = "Anthropic";

    public string Provider { get; set; } = LmStudio;
}
