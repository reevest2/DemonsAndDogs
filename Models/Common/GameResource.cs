using AppConstants;

namespace Models.Common;

public record GameResource : JsonResource
{
    public override string Kind => ResourceKinds.Game;
}
