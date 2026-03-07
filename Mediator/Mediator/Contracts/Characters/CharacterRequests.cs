using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts.Characters;

public record GetCharactersRequest : IRequest<IEnumerable<JsonResource>>;
public record GetCharacterRequest(string Id) : IRequest<JsonResource?>;
public record GetCharactersBySystemRequest(string SystemId) : IRequest<IEnumerable<JsonResource>>;
