using MediatR;
using Models.Narration;

namespace API.Services.Sessions.Contracts;

public record NarrateActionRequest(string SessionId) : IRequest<NarrationResult>;