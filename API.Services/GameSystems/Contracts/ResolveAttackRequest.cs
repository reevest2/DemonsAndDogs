using MediatR;
using Models;
using Models.GameSystems;

namespace API.Services.GameSystems.Contracts;

public record ResolveAttackRequest(string SystemId, AttackContext Context)
    : IRequest<Result<AttackResult>>;
