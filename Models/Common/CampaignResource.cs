using AppConstants;

namespace Models.Common;

public record CampaignResource : JsonResource
{
    public override string Kind => ResourceKinds.Campaign;
}
