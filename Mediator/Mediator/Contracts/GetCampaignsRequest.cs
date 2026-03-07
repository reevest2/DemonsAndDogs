using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts;

public record GetCampaignsRequest(string GameId) : IRequest<IEnumerable<JsonResource>>;
