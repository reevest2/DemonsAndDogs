using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;

namespace API.Services;

public class JsonResourceService(IResourceRepository<JsonResource> resourceRepository, ILogger<JsonResource> logger) 
    : ResourceService<JsonResource>(resourceRepository, logger), IJsonResourceService
{
    
}
public class SchemaResourceService(IResourceRepository<SchemaResource> resourceRepository, ILogger<SchemaResource> logger) 
    : ResourceService<SchemaResource>(resourceRepository, logger), ISchemaResourceService
{
}

public class PublishedSchemaResourceService(IResourceRepository<PublishedSchemaResource> resourceRepository, ILogger<PublishedSchemaResource> logger) 
    : ResourceService<PublishedSchemaResource>(resourceRepository, logger), IPublishedSchemaResourceService
{
}

public class DataResourceService(IResourceRepository<DataResource> resourceRepository, ILogger<DataResource> logger) 
    : ResourceService<DataResource>(resourceRepository, logger), IDataResourceService
{
}
