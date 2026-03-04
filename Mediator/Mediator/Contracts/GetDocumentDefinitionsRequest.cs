using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record GetDocumentDefinitionsRequest(string GameId) : IRequest<IEnumerable<JsonResource>>;
