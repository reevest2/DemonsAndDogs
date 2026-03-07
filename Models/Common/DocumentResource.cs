using AppConstants;

namespace Models.Common;

public record DocumentResource : JsonResource
{
    public override string Kind => ResourceKinds.Document;
}
