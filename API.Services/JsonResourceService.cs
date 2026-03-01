using API.Services.Abstraction;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;
using Models.Resources;

namespace API.Services;

public class JsonResourceService(IResourceRepository<JsonResource> resourceRepository, ILogger logger) 
    : ResourceService<JsonResource>(resourceRepository, logger), IJsonResourceService
{
    
}