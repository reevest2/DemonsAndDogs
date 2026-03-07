using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record CreateDocumentDefinitionRequest(string OwnerId, string GameId, string Name, string JsonContent) : IRequest<JsonResource>;
