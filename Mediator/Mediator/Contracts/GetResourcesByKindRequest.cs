using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record GetResourcesByKindRequest(string ResourceKind) : IRequest<IEnumerable<JsonResource>>;
