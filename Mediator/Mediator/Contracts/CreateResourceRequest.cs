using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record CreateResourceRequest(string OwnerId, string EntityId, string ResourceKind, string SchemaId, string JsonContent) : IRequest<JsonResource>;
