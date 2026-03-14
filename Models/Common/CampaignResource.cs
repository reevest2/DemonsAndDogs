using AppConstants;

namespace Models.Common;

public record CampaignResource : JsonResource
{
    public override string Kind { get; init; } = ResourceKinds.Campaign;
    public string Theme { get; init; } = "fantasy";
}
