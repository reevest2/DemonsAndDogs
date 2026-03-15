using Microsoft.AspNetCore.Mvc;
using API.Services.GameSystems;
using Models.GameSystems;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameSystemController(IGameSystemRegistry registry) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<GameSystemDescriptor>> GetAll()
    {
        var systems = registry.GetAll().Select(s => new GameSystemDescriptor(s.SystemId, s.DisplayName));
        return Ok(systems);
    }

    [HttpGet("{systemId}/schema")]
    public ActionResult<CharacterSheetSchema> GetSchema(string systemId)
    {
        var ruleBook = registry.Get(systemId);
        return Ok(ruleBook.GetCharacterSheetSchema());
    }
}
