using MediatR;
using API.Services.GameSystems;
using Models.GameSystems;
using Mediator.Mediator.Contracts.GameSystems;

namespace Mediator.Mediator.Handlers.GameSystems;

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
