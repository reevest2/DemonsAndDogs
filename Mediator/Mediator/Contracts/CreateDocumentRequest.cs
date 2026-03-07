using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record CreateDocumentRequest(string OwnerId, string GameId, string CampaignId, string DocumentDefinitionId, string Name, string JsonContent) : IRequest<JsonResource>;
