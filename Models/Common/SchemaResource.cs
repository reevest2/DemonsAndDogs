using AppConstants;

namespace Models.Common;

public record SchemaResource : JsonResource
{
    public override string Kind => ResourceKinds.Schema;
}
