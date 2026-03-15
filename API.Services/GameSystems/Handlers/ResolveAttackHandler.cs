using MediatR;
using Models;
using Models.GameSystems;
using API.Services.GameSystems.Contracts;

namespace API.Services.GameSystems.Handlers;

public class ResolveAttackHandler(IGameSystemRegistry registry)
    : IRequestHandler<ResolveAttackRequest, Result<AttackResult>>
{
    public Task<Result<AttackResult>> Handle(ResolveAttackRequest request, CancellationToken cancellationToken)
    {
        var ruleBookResult = registry.Get(request.SystemId);
        if (!ruleBookResult.IsSuccess)
            return Task.FromResult(Result<AttackResult>.Fail(ruleBookResult.Error!));

        var result = ruleBookResult.Value!.ResolveAttack(request.Context);
        return Task.FromResult(Result<AttackResult>.Ok(result));
    }
}
