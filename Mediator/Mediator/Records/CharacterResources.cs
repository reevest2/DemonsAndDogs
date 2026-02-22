using MediatR;
using Models.Resources;
using Models.Resources.Abstract;
using Models.Resources.Character;

namespace Mediator.Mediator.Records;

public record GetCharacterResourceQuery(string ResourceId) : IRequest<Resource<CharacterData>?>;
public record GetCharacterResourcesQuery() : IRequest<List<Resource<CharacterData>>>;
public record CreateCharacterResourceCommand(object Resource) : IRequest<Resource<CharacterData>>;
public record UpdateCharacterResourceCommand(string ResourceId, CharacterData Resource) : IRequest<Resource<CharacterData>>;
public record DeleteCharacterResourceCommand(string ResourceId) : IRequest;

public record GetCharacterTemplateResourceQuery() : IRequest<List<Resource<CharacterTemplateData>>>;
public sealed record GetCharacterTemplateResourcesQuery : IRequest<List<Resource<CharacterTemplateData>>>;
public record CreateCharacterTemplateResourceCommand(object Resource) : IRequest<Resource<CharacterTemplateData>>;
public record UpdateCharacterTemplateResourceCommand(string ResourceId, CharacterTemplateData Resource) : IRequest<Resource<CharacterTemplateData>>;
public record DeleteCharacterTemplateResourceCommand(string ResourceId) : IRequest;
