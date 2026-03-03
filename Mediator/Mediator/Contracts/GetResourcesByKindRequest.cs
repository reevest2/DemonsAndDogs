using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record GetResourcesByKindRequest(string ResourceKind) : IRequest<IEnumerable<JsonResource>>;
