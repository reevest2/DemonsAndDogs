using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts.Documents;

public record GetDocumentsByCampaignRequest(string CampaignId) : IRequest<IEnumerable<DocumentResource>>;
public record GetDocumentRequest(string Id) : IRequest<DocumentResource?>;
public record CreateDocumentRequest(DocumentResource Resource) : IRequest<DocumentResource>;
public record UpdateDocumentRequest(DocumentResource Resource) : IRequest<DocumentResource>;
public record DeleteDocumentRequest(string Id) : IRequest<Unit>;
