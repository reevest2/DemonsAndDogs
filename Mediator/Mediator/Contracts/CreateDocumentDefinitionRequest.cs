using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record CreateDocumentDefinitionRequest(string OwnerId, string GameId, string Name, string JsonContent) : IRequest<JsonResource>;
