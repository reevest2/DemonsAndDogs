using AppConstants;

namespace Models.Common;

public record CampaignResource : JsonResource
{
    public override string? ResourceKind { get; init; } = ResourceKinds.Campaign;
}
