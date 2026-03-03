using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record CreateSchemaRequest(string OwnerId, string Name, string ResourceKind, string JsonContent) : IRequest<JsonResource>;
