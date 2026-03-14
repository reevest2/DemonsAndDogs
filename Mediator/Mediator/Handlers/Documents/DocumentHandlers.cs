using MediatR;
using API.Services.Abstraction;
using Models.Common;
using Mediator.Mediator.Contracts.Documents;

namespace Mediator.Mediator.Handlers.Documents;

public class GetDocumentsByCampaignHandler(IDocumentService service) : IRequestHandler<GetDocumentsByCampaignRequest, IEnumerable<DocumentResource>>
{
    public async Task<IEnumerable<DocumentResource>> Handle(GetDocumentsByCampaignRequest request, CancellationToken cancellationToken)
    {
        return await service.GetByCampaignAsync(request.CampaignId, cancellationToken);
    }
}

public class GetDocumentHandler(IDocumentService service) : IRequestHandler<GetDocumentRequest, DocumentResource?>
{
    public async Task<DocumentResource?> Handle(GetDocumentRequest request, CancellationToken cancellationToken)
    {
        return await service.GetByIdAsync(request.Id, cancellationToken);
    }
}

public class CreateDocumentHandler(IDocumentService service) : IRequestHandler<CreateDocumentRequest, DocumentResource>
{
    public async Task<DocumentResource> Handle(CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        return await service.CreateAsync(request.Resource, cancellationToken);
    }
}

public class UpdateDocumentHandler(IDocumentService service) : IRequestHandler<UpdateDocumentRequest, DocumentResource>
{
    public async Task<DocumentResource> Handle(UpdateDocumentRequest request, CancellationToken cancellationToken)
    {
        return await service.UpdateAsync(request.Resource, cancellationToken);
    }
}

public class DeleteDocumentHandler(IDocumentService service) : IRequestHandler<DeleteDocumentRequest, Unit>
{
    public async Task<Unit> Handle(DeleteDocumentRequest request, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
