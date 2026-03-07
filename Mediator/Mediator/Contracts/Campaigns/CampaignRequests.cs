using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts.Campaigns;

public record GetCampaignsRequest : IRequest<IEnumerable<CampaignResource>>;
public record GetCampaignRequest(string Id) : IRequest<CampaignResource?>;
