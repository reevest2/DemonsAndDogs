public interface IApiClient
{
    Task<T> Get<T>(string path, CancellationToken ct);
    Task<TOut> Post<TIn, TOut>(string path, TIn body, CancellationToken ct);
    Task Put<TIn>(string path, TIn body, CancellationToken ct);
    Task Delete(string path, CancellationToken ct);
}
