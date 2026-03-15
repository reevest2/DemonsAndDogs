using MediatR;
using Models;
using Models.Narration;

namespace API.Services.Sessions.Contracts;

public record NarrateActionRequest(string SessionId) : IRequest<Result<NarrationResult>>;
