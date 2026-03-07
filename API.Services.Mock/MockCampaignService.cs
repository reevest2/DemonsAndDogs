using API.Services.Abstraction;
using Models.Common;
using System.Text.Json;
using AppConstants;

namespace API.Services.Mock;

public class MockCampaignService : ICampaignService
{
    private static readonly List<JsonResource> _campaigns = new()
    {
        new CampaignResource
        {
            Id = "mock-campaign-1",
            EntityId = "Lost Mine of Phandelver",
            GameId = "dnd5e",
            ResourceKind = ResourceKinds.Campaign,
            Data = JsonSerializer.Deserialize<JsonElement>(@"{ ""name"": ""Lost Mine of Phandelver"", ""description"": ""A classic D&D 5e starter adventure."" }")
        },
        new CampaignResource
        {
            Id = "mock-campaign-2",
            EntityId = "Curse of Strahd",
            GameId = "dnd5e",
            ResourceKind = ResourceKinds.Campaign,
            Data = JsonSerializer.Deserialize<JsonElement>(@"{ ""name"": ""Curse of Strahd"", ""description"": ""A gothic horror adventure in Barovia."" }")
        }
    };

    public Task<IEnumerable<JsonResource>> GetAllAsync() => Task.FromResult<IEnumerable<JsonResource>>(_campaigns);

    public Task<JsonResource?> GetByIdAsync(string id) => Task.FromResult(_campaigns.FirstOrDefault(c => c.Id == id));
}
