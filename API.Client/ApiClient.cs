using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace API.Client;

public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<T> Get<T>(string path, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<T>(path, ct);
        if (result is null) throw new HttpRequestException($"Null response for GET {path}");
        return result;
    }

    public async IAsyncEnumerable<string> GetStream(string path, [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (true)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line == null) break;
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("data: "))
            {
                var data = line["data: ".Length..];
                if (data == "[DONE]") break;

                if (data.StartsWith("[ERROR]"))
                {
                    yield return data;
                    continue;
                }

                if (!string.IsNullOrEmpty(data))
                {
                    yield return data;
                }
            }
        }
    }

    public async Task<TOut> Post<TIn, TOut>(string path, TIn body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(path, body, ct);
        res.EnsureSuccessStatusCode();
        var result = await res.Content.ReadFromJsonAsync<TOut>(cancellationToken: ct);
        if (result is null) throw new HttpRequestException($"Null response for POST {path}");
        return result;
    }

    public async Task Post<TIn>(string path, TIn body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(path, body, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task Put<TIn>(string path, TIn body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync(path, body, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task Delete(string path, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync(path, ct);
        res.EnsureSuccessStatusCode();
    }
}
