using AppConstants;
using MediatR;
using Models.Resources.Character;

namespace Mediator.Mediator.Records;

public record SaveResourceCommand(string ResourceName, object Resource) : IRequest;


public sealed class SaveResourceCommandHandler(IMediator mediator) : IRequestHandler<SaveResourceCommand>
{
    public async Task Handle(SaveResourceCommand request, CancellationToken cancellationToken)
    {
        if (request.ResourceName.Equals(ResourceKeys.CharacterTemplateResources, StringComparison.OrdinalIgnoreCase))
        {
            await mediator.Send(new CreateCharacterTemplateResourceCommand(request.Resource), cancellationToken);
            return;
        }

        if (request.ResourceName.Equals(ResourceKeys.CharacterResources, StringComparison.OrdinalIgnoreCase))
        {
            await mediator.Send(new CreateCharacterResourceCommand(request.Resource), cancellationToken);
            return;
        }

        throw new InvalidOperationException($"No save mapping for resource '{request.ResourceName}'.");
    }
}