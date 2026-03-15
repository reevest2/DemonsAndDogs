using API.Services.Campaigns;
using Mediator.Mediator.Contracts.Campaigns;
using Mediator.Mediator.Handlers.Campaigns;
using Models.Common;

namespace DemonsAndDogs.API.Tests.Handlers;

// ---------------------------------------------------------------------------
// Fakes
// ---------------------------------------------------------------------------

file class FakeCampaignService : ICampaignService
{
    public List<CampaignResource> Campaigns { get; set; } = [];

    public Task<IEnumerable<CampaignResource>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<CampaignResource>>(Campaigns);

    public Task<CampaignResource?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Campaigns.FirstOrDefault(c => c.Id == id));
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

/// <summary>
/// Unit tests for campaign MediatR handlers.
/// These handlers delegate directly to ICampaignService with no additional logic.
/// </summary>
public class CampaignHandlersTests
{
    // -----------------------------------------------------------------------
    // GetCampaignsHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCampaigns_WithCampaigns_ReturnsAll()
    {
        var service = new FakeCampaignService
        {
            Campaigns = [new() { Id = "c1", EntityId = "Dark Crusade" }, new() { Id = "c2", EntityId = "Lost Mine" }]
        };
        var handler = new GetCampaignsHandler(service);

        var result = await handler.Handle(new GetCampaignsRequest(), default);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCampaigns_NoCampaigns_ReturnsEmpty()
    {
        var handler = new GetCampaignsHandler(new FakeCampaignService());

        var result = await handler.Handle(new GetCampaignsRequest(), default);

        Assert.Empty(result);
    }

    // -----------------------------------------------------------------------
    // GetCampaignHandler
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetCampaign_ExistingId_ReturnsCampaign()
    {
        var service = new FakeCampaignService
        {
            Campaigns = [new() { Id = "c1", EntityId = "Dark Crusade" }]
        };
        var handler = new GetCampaignHandler(service);

        var result = await handler.Handle(new GetCampaignRequest("c1"), default);

        Assert.NotNull(result);
        Assert.Equal("Dark Crusade", result.EntityId);
    }

    [Fact]
    public async Task GetCampaign_UnknownId_ReturnsNull()
    {
        var handler = new GetCampaignHandler(new FakeCampaignService());

        var result = await handler.Handle(new GetCampaignRequest("unknown"), default);

        Assert.Null(result);
    }
}
