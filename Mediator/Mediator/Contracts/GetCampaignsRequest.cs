using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Contracts;

public record GetCampaignsRequest(string GameId) : IRequest<IEnumerable<JsonResource>>;
