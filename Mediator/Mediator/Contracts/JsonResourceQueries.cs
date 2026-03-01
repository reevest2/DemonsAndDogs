using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record GetJsonResourcesQuery(string OwnerId) : IRequest<List<JsonResource>>;
public record GetJsonResourceByIdQuery(string OwnerId, string ResourceId) : IRequest<JsonResource>;
