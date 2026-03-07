using MediatR;
using API.Services.Abstraction;
using Models.Common;
using Mediator.Mediator.Contracts.Characters;

namespace Mediator.Mediator.Handlers.Characters;

public class GetCharactersHandler(ICharacterService service) : IRequestHandler<GetCharactersRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetCharactersRequest request, CancellationToken cancellationToken)
    {
        return await service.GetAllAsync();
    }
}

public class GetCharacterHandler(ICharacterService service) : IRequestHandler<GetCharacterRequest, JsonResource?>
{
    public async Task<JsonResource?> Handle(GetCharacterRequest request, CancellationToken cancellationToken)
    {
        return await service.GetByIdAsync(request.Id);
    }
}

public class GetCharactersBySystemHandler(ICharacterService service) : IRequestHandler<GetCharactersBySystemRequest, IEnumerable<JsonResource>>
{
    public async Task<IEnumerable<JsonResource>> Handle(GetCharactersBySystemRequest request, CancellationToken cancellationToken)
    {
        return await service.GetBySystemIdAsync(request.SystemId);
    }
}
