namespace API.Client;

public interface IApiClient
{
    Task<T> Get<T>(string path, CancellationToken ct = default);
    IAsyncEnumerable<string> GetStream(string path, CancellationToken ct = default);
    Task<TOut> Post<TIn, TOut>(string path, TIn body, CancellationToken ct = default);
    Task Post<TIn>(string path, TIn body, CancellationToken ct = default);
    Task Put<TIn>(string path, TIn body, CancellationToken ct = default);
    Task Delete(string path, CancellationToken ct = default);
}
