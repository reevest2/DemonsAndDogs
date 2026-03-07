using MediatR;
using Models.GameSystems;

namespace Mediator.Mediator.Contracts.GameSystems;

public record GetCharacterSheetSchemaRequest(string SystemId) : IRequest<CharacterSheetSchema>;
