using MediatR;
using Models.Session;

namespace Mediator.Mediator.Contracts.Session;

public record GetSessionRequest(string SessionId) : IRequest<SessionState>;
