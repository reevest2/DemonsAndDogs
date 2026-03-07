using MediatR;
using Models.Session;

namespace Mediator.Mediator.Contracts.Session;

public record StartSessionRequest(string CharacterName, string SystemId)
    : IRequest<SessionState>;
