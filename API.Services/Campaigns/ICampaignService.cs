using Models.Common;

namespace API.Services.Campaigns;

public interface ICampaignService
{
    Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CampaignResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
