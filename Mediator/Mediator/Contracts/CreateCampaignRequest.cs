using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record CreateCampaignRequest(string OwnerId, string GameId, string Name, string Description) : IRequest<JsonResource>;
