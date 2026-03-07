using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record CreateCampaignRequest(string OwnerId, string GameId, string Name, string Description) : IRequest<JsonResource>;
