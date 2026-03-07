using MediatR;
using API.Services.Abstraction;
using Models.Common;
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
