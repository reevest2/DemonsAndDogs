using API.Services.Abstraction;
using DataAccess.Abstraction;
using Models.Common;

namespace API.Services.Campaign;

public class JsonCampaignService(IJsonResourceRepository repository) : ICampaignService
{
    public async Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var results = await repository.QueryAsync(q => q.OfType<CampaignResource>());
        return results.Cast<CampaignResource>();
    }

    public async Task<CampaignResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var results = await repository.QueryAsync(q => q.OfType<CampaignResource>().Where(r => r.Id == id));
        return results.Cast<CampaignResource>().FirstOrDefault();
    }
}
