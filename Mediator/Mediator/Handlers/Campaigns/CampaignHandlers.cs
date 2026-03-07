using MediatR;
using API.Services.Abstraction;
using Models.Common;
using Mediator.Mediator.Contracts.Campaigns;

namespace Mediator.Mediator.Handlers.Campaigns;

public class GetCampaignsHandler(ICampaignService service) : IRequestHandler<GetCampaignsRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetCampaignsRequest request, CancellationToken cancellationToken)
    {
        return await service.GetAllAsync();
    }
}

public class GetCampaignHandler(ICampaignService service) : IRequestHandler<GetCampaignRequest, JsonResource?>
{
    public async Task<JsonResource?> Handle(GetCampaignRequest request, CancellationToken cancellationToken)
    {
        return await service.GetByIdAsync(request.Id);
    }
}
