using MediatR;
using API.Services.Abstraction;
using Mediator.Mediator.Contracts.GameSystems;
using Models.GameSystems;

namespace Mediator.Mediator.Handlers.GameSystems;

public class GetCharacterSheetSchemaHandler(IGameSystemRegistry registry)
    : IRequestHandler<GetCharacterSheetSchemaRequest, CharacterSheetSchema>
{
    public Task<CharacterSheetSchema> Handle(GetCharacterSheetSchemaRequest request, CancellationToken cancellationToken)
    {
        var ruleBook = registry.Get(request.SystemId);
        return Task.FromResult(ruleBook.GetCharacterSheetSchema());
    }
}
