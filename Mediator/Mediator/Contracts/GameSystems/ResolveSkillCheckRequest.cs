using MediatR;
using Models.GameSystems;

namespace Mediator.Mediator.Contracts.GameSystems;

public record ResolveSkillCheckRequest(string SystemId, SkillCheckContext Context)
    : IRequest<CheckResult>;
