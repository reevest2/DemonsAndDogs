using MediatR;
using Models.Common;

namespace Mediator.Mediator.Contracts.Characters;

public record GetCharactersRequest : IRequest<IEnumerable<CharacterResource>>;
public record GetCharacterRequest(string Id) : IRequest<CharacterResource?>;
public record GetCharactersBySystemRequest(string SystemId) : IRequest<IEnumerable<CharacterResource>>;
public record GetCharacterStatsRequest(string CharacterId) : IRequest<IReadOnlyDictionary<string, int>>;
