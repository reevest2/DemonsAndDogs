using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record GetGamesRequest() : IRequest<IEnumerable<JsonResource>>;
