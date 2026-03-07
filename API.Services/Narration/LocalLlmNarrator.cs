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
            input = prompt, 
            stream = true
        };

        var url = _options.BaseUrl.TrimEnd('/') + "/v1/chat";
        
        return new NarrationResult(
            Text: string.Empty,
            TokenStream: StreamTokensAsync(url, request));
    }

    private async IAsyncEnumerable<string> StreamTokensAsync(string url, object request)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line == "data: [DONE]") break;

            if (line.StartsWith("data: "))
            {
                var json2 = line.Substring(6);
                Console.WriteLine($"[LLM RAW] {json2}");
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