using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record GetDocumentsRequest(string GameId, string? CampaignId = null) : IRequest<IEnumerable<JsonResource>>;
