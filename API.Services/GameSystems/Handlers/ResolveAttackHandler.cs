using MediatR;
using Models.GameSystems;
using API.Services.GameSystems.Contracts;

namespace API.Services.GameSystems.Handlers;

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
