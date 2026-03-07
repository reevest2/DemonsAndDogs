using MediatR;
using Models.GameSystems;

namespace Mediator.Mediator.Contracts.GameSystems;

public record GetGameSystemsRequest() : IRequest<IEnumerable<GameSystemDescriptor>>;
