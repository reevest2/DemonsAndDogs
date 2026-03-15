using API.Services.Campaigns;
using Models.Common;
using System.Text.Json;
using AppConstants;

namespace API.Services.Mock;

public class MockCampaignService : ICampaignService
{
    private static readonly List<CampaignResource> _campaigns = new()
    {
        new()
        {
            Id = "mock-campaign-1",
            EntityId = "Lost Mine of Phandelver",
            GameId = "dnd5e",
            ResourceKind = ResourceKinds.Campaign,
            Data = JsonSerializer.Deserialize<JsonElement>(@"{ ""name"": ""Lost Mine of Phandelver"", ""description"": ""A classic D&D 5e starter adventure."" }")
        },
        new()
        {
            Id = "mock-campaign-2",
            EntityId = "Curse of Strahd",
            GameId = "dnd5e",
            ResourceKind = ResourceKinds.Campaign,
            Data = JsonSerializer.Deserialize<JsonElement>(@"{ ""name"": ""Curse of Strahd"", ""description"": ""A gothic horror adventure in Barovia."" }")
        }
    };

    public Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<CampaignResource>>(_campaigns);

    public Task<CampaignResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_campaigns.FirstOrDefault(c => c.Id == id));
}
