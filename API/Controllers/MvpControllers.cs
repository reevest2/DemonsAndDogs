using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models.Resources;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchemaResourceController(ISchemaResourceService service) : ResourceControllerBase<SchemaResource, ISchemaResourceService>(service)
{
}

[ApiController]
[Route("api/[controller]")]
public class PublishedSchemaResourceController(IPublishedSchemaResourceService service) : ResourceControllerBase<PublishedSchemaResource, IPublishedSchemaResourceService>(service)
{
}

[ApiController]
[Route("api/[controller]")]
public class DataResourceController(IDataResourceService service) : ResourceControllerBase<DataResource, IDataResourceService>(service)
{
}
