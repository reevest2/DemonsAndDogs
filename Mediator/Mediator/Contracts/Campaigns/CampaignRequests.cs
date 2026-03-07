using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts.Campaigns;

public record GetCampaignsRequest : IRequest<IEnumerable<JsonResource>>;
public record GetCampaignRequest(string Id) : IRequest<JsonResource?>;
