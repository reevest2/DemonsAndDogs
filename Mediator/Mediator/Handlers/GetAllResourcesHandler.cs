using Mediator.Mediator.Records;
using MediatR;

namespace Mediator.Mediator.Handlers;

using MediatR;
using AppConstants;
using Models.Resources;

public sealed class GetAllResourcesQueryHandler(IMediator mediator) : IRequestHandler<GetAllResourcesQuery, object>
{
    public async Task<object> Handle(GetAllResourcesQuery request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException($"No get-all mapping for resource '{request.ResourceName}'.");
    }
}