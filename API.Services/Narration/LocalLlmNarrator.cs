using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using AppConstants;
using Microsoft.Extensions.Options;
using Models.Interfaces;
using Models.Narration;

namespace API.Services.Narration;

public class LocalLlmNarrator(HttpClient httpClient, IOptions<LocalLlmOptions> options) : INarrator
{
    private readonly LocalLlmOptions _options = options.Value;

    public Task<NarrationResult> NarrateAsync(GameEvent gameEvent, string? Tone = NarrationTones.Neutral, CancellationToken cancellationToken = default)
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

        var url = _options.BaseUrl.TrimEnd('/') + LmStudioApiEndpoints.Chat;

        return Task.FromResult(new NarrationResult(
            Text: string.Empty,
            TokenStream: StreamTokensAsync(url, request, cancellationToken)));
    }

    private async IAsyncEnumerable<string> StreamTokensAsync(string url, object request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(request)
        };

        using var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line == LmStudioEventTypes.SseDone) break;

            if (line.StartsWith(LmStudioEventTypes.SsePrefix))
            {
                var json = line.Substring(LmStudioEventTypes.SsePrefix.Length);
                JsonNode? node;
                try
                {
                    node = JsonNode.Parse(json);
                }
                catch
                {
                    continue;
                }

                var token = node?[LmStudioEventTypes.TypeField]?.ToString() == LmStudioEventTypes.MessageDelta
                    ? node?[LmStudioEventTypes.ContentField]?.ToString()
                    : null;
                if (!string.IsNullOrEmpty(token))
                {
                    yield return token;
                }
            }
        }
    }
}
