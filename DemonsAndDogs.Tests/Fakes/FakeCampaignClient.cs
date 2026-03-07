using API.Client.Abstraction;
using Models.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemonsAndDogs.Tests.Fakes;

public class FakeCampaignClient : ICampaignClient
{
    public IEnumerable<CampaignResource>? Campaigns { get; set; }
    public Task<IEnumerable<CampaignResource>>? CustomTask { get; set; }

    public Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken ct = default)
    {
        return CustomTask ?? Task.FromResult(Campaigns ?? new List<CampaignResource>());
    }

    public Task<CampaignResource?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }
}
