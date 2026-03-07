using Models.Common;

namespace API.Client.Abstraction;

public interface ICampaignClient
{
    Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken ct = default);
    Task<CampaignResource?> GetByIdAsync(string id, CancellationToken ct = default);
}
