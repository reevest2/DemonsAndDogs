using MediatR;
using Models;
using Models.GameSystems;
using API.Services.GameSystems.Contracts;

namespace API.Services.GameSystems.Handlers;

public class ResolveSkillCheckHandler(IGameSystemRegistry registry)
    : IRequestHandler<ResolveSkillCheckRequest, Result<CheckResult>>
{
    public Task<Result<CheckResult>> Handle(ResolveSkillCheckRequest request, CancellationToken cancellationToken)
    {
        var ruleBookResult = registry.Get(request.SystemId);
        if (!ruleBookResult.IsSuccess)
            return Task.FromResult(Result<CheckResult>.Fail(ruleBookResult.Error!));

        var result = ruleBookResult.Value!.ResolveSkillCheck(request.Context);
        return Task.FromResult(Result<CheckResult>.Ok(result));
    }
}
