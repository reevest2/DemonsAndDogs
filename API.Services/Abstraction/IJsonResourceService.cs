using Models.Resources;

namespace API.Services.Abstraction;

public interface IJsonResourceService : IResourceService<JsonResource>
{
}

public interface ISchemaResourceService : IResourceService<SchemaResource>
{
}
public interface IPublishedSchemaResourceService : IResourceService<PublishedSchemaResource>
{
}

public interface IDataResourceService : IResourceService<DataResource>
{
}
