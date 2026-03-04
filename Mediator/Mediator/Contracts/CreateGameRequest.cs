using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record CreateGameRequest(string OwnerId, string Name, string Description) : IRequest<JsonResource>;
