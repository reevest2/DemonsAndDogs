using MediatR;
using API.Services.Campaigns;
using Models.Common;
using Mediator.Mediator.Contracts.Campaigns;

namespace Mediator.Mediator.Handlers.Campaigns;

public class GetCampaignsHandler(ICampaignService service) : IRequestHandler<GetCampaignsRequest, IEnumerable<CampaignResource>>
{
    public async Task<IEnumerable<CampaignResource>> Handle(GetCampaignsRequest request, CancellationToken cancellationToken)
    {
        return await service.GetAllAsync();
    }
}

public class GetCampaignHandler(ICampaignService service) : IRequestHandler<GetCampaignRequest, CampaignResource?>
{
    public async Task<CampaignResource?> Handle(GetCampaignRequest request, CancellationToken cancellationToken)
    {
        return await service.GetByIdAsync(request.Id);
    }
}
