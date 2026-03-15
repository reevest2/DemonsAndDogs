using MediatR;
using Models.Session;

namespace API.Services.Sessions.Contracts;

public record StartSessionRequest(string CharacterId, string CharacterName, string SystemId)
    : IRequest<SessionState>;
