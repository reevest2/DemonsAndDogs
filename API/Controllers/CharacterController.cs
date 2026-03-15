using Microsoft.AspNetCore.Mvc;
using Models.Common;
using API.Services.Characters;
using API.Services.GameSystems;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController(ICharacterService service, IGameSystemRegistry registry) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResource>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterResource>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("system/{systemId}")]
    public async Task<ActionResult<IEnumerable<CharacterResource>>> GetBySystem(string systemId, CancellationToken cancellationToken)
    {
        return Ok(await service.GetBySystemIdAsync(systemId, cancellationToken));
    }

    [HttpGet("{id}/stats")]
    public async Task<ActionResult<IReadOnlyDictionary<string, int>>> GetStats(string id, CancellationToken cancellationToken)
    {
        var character = await service.GetByIdAsync(id, cancellationToken);
        if (character == null || string.IsNullOrEmpty(character.GameId))
            return Ok(new Dictionary<string, int>());

        var ruleBook = registry.Get(character.GameId);
        return Ok(ruleBook.ExtractStats(character.Data));
    }
}
