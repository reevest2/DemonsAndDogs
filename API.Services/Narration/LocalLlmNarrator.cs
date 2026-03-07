using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Models.Interfaces;
using Models.Narration;
namespace API.Services.Narration;

public class LocalLlmNarrator(HttpClient httpClient, IOptions<LocalLlmOptions> options) : INarrator
{
    private readonly LocalLlmOptions _options = options.Value;

    public async Task<NarrationResult> NarrateAsync(GameEvent gameEvent, string? Tone = "neutral")
    {
        var metadataStr = gameEvent.Metadata != null 
            ? string.Join(", ", gameEvent.Metadata.Select(m => $"{m.Key}: {m.Value}")) 
            : "None";

        var prompt = $"Character: {gameEvent.SubjectId}\nAction: {gameEvent.EventType}\nOutcome: {gameEvent.Description}\nDetails: {metadataStr}\nTone: {Tone}";
        
        var request = new
        {
            model = _options.ModelId,
            messages = new[]
            {
                new { role = "system", content = _options.SystemPrompt },
                new { role = "user", content = prompt }
            },
            stream = true
        };

        var url = _options.BaseUrl + "/v1/chat/completions";
        var response = await httpClient.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        
        return new NarrationResult(
            Text: string.Empty,
            TokenStream: StreamTokensAsync(stream));
    }

    private async IAsyncEnumerable<string> StreamTokensAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line == "data: [DONE]") break;

            if (line.StartsWith("data: "))
            {
                var json = line.Substring(6);
                JsonNode? node;
                try 
                {
                    node = JsonNode.Parse(json);
                }
                catch
                {
                    continue;
                }
                
                var token = node?["choices"]?[0]?["delta"]?["content"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    yield return token;
                }
            }
        }
    }
}