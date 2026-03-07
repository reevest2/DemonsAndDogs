using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record GetDocumentDefinitionsRequest(string GameId) : IRequest<IEnumerable<JsonResource>>;
