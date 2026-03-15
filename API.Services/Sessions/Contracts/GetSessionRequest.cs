using MediatR;
using Models;
using Models.Session;

namespace API.Services.Sessions.Contracts;

public record GetSessionRequest(string SessionId) : IRequest<Result<SessionState>>;
