using MediatR;
using Models.GameSystems;

namespace API.Services.GameSystems.Contracts;

public record ResolveAttackRequest(string SystemId, AttackContext Context)
    : IRequest<AttackResult>;
