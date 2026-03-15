using MediatR;
using API.Services.GameSystems;
using Models.GameSystems;
using Mediator.Mediator.Contracts.GameSystems;

namespace Mediator.Mediator.Handlers.GameSystems;

public class ResolveAttackHandler(IGameSystemRegistry registry)
    : IRequestHandler<ResolveAttackRequest, AttackResult>
{
    public Task<AttackResult> Handle(ResolveAttackRequest request, CancellationToken cancellationToken)
    {
        var ruleBook = registry.Get(request.SystemId);
        var result = ruleBook.ResolveAttack(request.Context);
        return Task.FromResult(result);
    }
}
