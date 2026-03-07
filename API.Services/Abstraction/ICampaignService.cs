using Models.Common;

namespace API.Services.Abstraction;

public interface ICampaignService
{
    Task<IEnumerable<CampaignResource>> GetAllAsync();
    Task<CampaignResource?> GetByIdAsync(string id);
}