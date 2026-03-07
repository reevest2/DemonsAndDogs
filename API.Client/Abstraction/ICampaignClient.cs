using Models.Common;

namespace API.Client.Abstraction;

public interface ICampaignClient
{
    Task<IEnumerable<JsonResource>> GetAllAsync(CancellationToken ct = default);
    Task<JsonResource?> GetByIdAsync(string id, CancellationToken ct = default);
}
