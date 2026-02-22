using Mediator.Mediator.Records;
using MediatR;

namespace Mediator.Mediator.Handlers;

using MediatR;
using AppConstants;
using Models.Character;
using Models.Resources;

public sealed class GetAllResourcesQueryHandler(IMediator mediator) : IRequestHandler<GetAllResourcesQuery, object>
{
    public async Task<object> Handle(GetAllResourcesQuery request, CancellationToken cancellationToken)
    {
        if (request.ResourceName.Equals(ResourceKeys.CharacterTemplateResources, StringComparison.OrdinalIgnoreCase))
        {
            var items = await mediator.Send(new GetCharacterTemplateResourceQuery(), cancellationToken);
            return items;
        }

        if (request.ResourceName.Equals(ResourceKeys.CharacterResources, StringComparison.OrdinalIgnoreCase))
        {
            var items = await mediator.Send(new GetCharacterResourcesQuery(), cancellationToken);
            return items;
        }

        throw new InvalidOperationException($"No get-all mapping for resource '{request.ResourceName}'.");
    }
}