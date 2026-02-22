using MediatR;

namespace Mediator.Mediator.Records;

public record GetAllResourcesQuery(string ResourceName) : IRequest<object>;