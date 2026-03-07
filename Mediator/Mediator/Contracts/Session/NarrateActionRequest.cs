using MediatR;
using Models.Narration;

namespace Mediator.Mediator.Contracts.Session;

public record NarrateActionRequest(string SessionId) : IRequest<NarrationResult>;