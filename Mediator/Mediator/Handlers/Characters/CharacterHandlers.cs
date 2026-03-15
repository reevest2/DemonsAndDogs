using MediatR;
using API.Services.Characters;
using API.Services.GameSystems;
using Models.Common;
using Models.Interfaces;
using Mediator.Mediator.Contracts.Characters;

namespace Mediator.Mediator.Handlers.Characters;

public class GetCharactersHandler(ICharacterService service) : IRequestHandler<GetCharactersRequest, IEnumerable<CharacterResource>>
{
    public async Task<IEnumerable<CharacterResource>> Handle(GetCharactersRequest request, CancellationToken cancellationToken)
    {
        return await service.GetAllAsync();
    }
}

public class GetCharacterHandler(ICharacterService service) : IRequestHandler<GetCharacterRequest, CharacterResource?>
{
    public async Task<CharacterResource?> Handle(GetCharacterRequest request, CancellationToken cancellationToken)
    {
        return await service.GetByIdAsync(request.Id);
    }
}

public class GetCharactersBySystemHandler(ICharacterService service) : IRequestHandler<GetCharactersBySystemRequest, IEnumerable<CharacterResource>>
{
    public async Task<IEnumerable<CharacterResource>> Handle(GetCharactersBySystemRequest request, CancellationToken cancellationToken)
    {
        return await service.GetBySystemIdAsync(request.SystemId);
    }
}

public class GetCharacterStatsHandler(ICharacterService characterService, IGameSystemRegistry registry)
    : IRequestHandler<GetCharacterStatsRequest, IReadOnlyDictionary<string, int>>
{
    public async Task<IReadOnlyDictionary<string, int>> Handle(GetCharacterStatsRequest request, CancellationToken cancellationToken)
    {
        var character = await characterService.GetByIdAsync(request.CharacterId, cancellationToken);
        if (character == null || string.IsNullOrEmpty(character.GameId))
            return new Dictionary<string, int>();

        var ruleBook = registry.Get(character.GameId);
        return ruleBook.ExtractStats(character.Data);
    }
}
