using MediatR;
using Models.Session;

namespace API.Services.Sessions.Contracts;

public record GetSessionRequest(string SessionId) : IRequest<SessionState>;
