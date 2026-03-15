using MediatR;
using Models.GameSystems;

namespace API.Services.GameSystems.Contracts;

public record ResolveSkillCheckRequest(string SystemId, SkillCheckContext Context)
    : IRequest<CheckResult>;
