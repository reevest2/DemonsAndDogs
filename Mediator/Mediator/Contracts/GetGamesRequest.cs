using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record GetGamesRequest() : IRequest<IEnumerable<JsonResource>>;
