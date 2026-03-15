using MediatR;
using API.Services.GameSystems;
using Mediator.Mediator.Contracts.GameSystems;
using Models.GameSystems;

namespace Mediator.Mediator.Handlers.GameSystems;

public class GetGameSystemsHandler(IGameSystemRegistry registry) 
    : IRequestHandler<GetGameSystemsRequest, IEnumerable<GameSystemDescriptor>>
{
    public Task<IEnumerable<GameSystemDescriptor>> Handle(GetGameSystemsRequest request, CancellationToken cancellationToken)
    {
        var systems = registry.GetAll().Select(s => new GameSystemDescriptor(s.SystemId, s.DisplayName));
        return Task.FromResult(systems);
    }
}
