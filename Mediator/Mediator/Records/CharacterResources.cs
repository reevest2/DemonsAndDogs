using MediatR;
using Models.Resources;

namespace Mediator.Mediator.Records;

public record GetCharacterResourceQuery(string ResourceId) : IRequest<CharacterData?>;
public record GetCharacterResourcesQuery() : IRequest<List<CharacterData>>;
public record CreateCharacterResourceCommand(CharacterData Resource) : IRequest<CharacterData>;
public record UpdateCharacterResourceCommand(string ResourceId, CharacterData Resource) : IRequest<CharacterData>;
public record DeleteCharacterResourceCommand(string ResourceId) : IRequest;

public record GetCharacterTemplateResourceQuery(string ResourceId) : IRequest<CharacterTemplateData?>;
public record GetCharacterTemplateResourcesQuery() : IRequest<List<CharacterTemplateData>>;
public record CreateCharacterTemplateResourceCommand(CharacterTemplateData Resource) : IRequest<CharacterTemplateData>;
public record UpdateCharacterTemplateResourceCommand(string ResourceId, CharacterTemplateData Resource) : IRequest<CharacterTemplateData>;
public record DeleteCharacterTemplateResourceCommand(string ResourceId) : IRequest;
