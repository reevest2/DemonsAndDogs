using System.Net.Http.Json;

public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<T> Get<T>(string path, CancellationToken ct)
    {
        var result = await _http.GetFromJsonAsync<T>(path, ct);
        if (result is null) throw new HttpRequestException($"Null response for GET {path}");
        return result;
    }

    public async Task<TOut> Post<TIn, TOut>(string path, TIn body, CancellationToken ct)
    {
        var res = await _http.PostAsJsonAsync(path, body, ct);
        res.EnsureSuccessStatusCode();
        var result = await res.Content.ReadFromJsonAsync<TOut>(cancellationToken: ct);
        if (result is null) throw new HttpRequestException($"Null response for POST {path}");
        return result;
    }

    public async Task Put<TIn>(string path, TIn body, CancellationToken ct)
    {
        var res = await _http.PutAsJsonAsync(path, body, ct);
        res.EnsureSuccessStatusCode();
    }

    public async Task Delete(string path, CancellationToken ct)
    {
        var res = await _http.DeleteAsync(path, ct);
        res.EnsureSuccessStatusCode();
    }
}