using MediatR;
using Models.GameSystems;
using API.Services.GameSystems.Contracts;

namespace API.Services.GameSystems.Handlers;

public class ResolveSkillCheckHandler(IGameSystemRegistry registry)
    : IRequestHandler<ResolveSkillCheckRequest, CheckResult>
{
    public Task<CheckResult> Handle(ResolveSkillCheckRequest request, CancellationToken cancellationToken)
    {
        var ruleBook = registry.Get(request.SystemId);
        var result = ruleBook.ResolveSkillCheck(request.Context);
        return Task.FromResult(result);
    }
}
