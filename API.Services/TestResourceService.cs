using API.Services.Abstraction;
using Models;
using DataAccess.Abstraction;
using Microsoft.Extensions.Logging;

namespace API.Services;

public class TestResourceService : IResourceService.ResourceService<TestResource>, ITestResourceService
{
    public TestResourceService(IResourceRepository<TestResource> resourceRepository, ILogger<TestResourceService> logger)
        : base(resourceRepository, logger)
    {
    }
}
