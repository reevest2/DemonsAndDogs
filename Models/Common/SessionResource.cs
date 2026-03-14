using AppConstants;

namespace Models.Common;

public record SessionResource : JsonResource
{
    public override string Kind => ResourceKinds.Session;
}
