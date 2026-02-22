using AppConstants;
using MediatR;
using Models.Resources.Character;

namespace Mediator.Mediator.Records;

public record SaveResourceCommand(string ResourceName, object Resource) : IRequest;


public sealed class SaveResourceCommandHandler(IMediator mediator) : IRequestHandler<SaveResourceCommand>
{
    public async Task Handle(SaveResourceCommand request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException($"No save mapping for resource '{request.ResourceName}'.");
    }
}