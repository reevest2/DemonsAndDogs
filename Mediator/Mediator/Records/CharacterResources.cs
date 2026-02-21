using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Records;

public record GetCharacterResourceQuery(string ResourceId) : IRequest<CharacterResource?>;
public record GetCharacterResourcesQuery() : IRequest<List<CharacterResource>>;
public record CreateCharacterResourceCommand(CharacterResource Resource) : IRequest<CharacterResource>;
public record UpdateCharacterResourceCommand(string ResourceId, CharacterResource Resource) : IRequest<CharacterResource>;
public record DeleteCharacterResourceCommand(string ResourceId) : IRequest;
