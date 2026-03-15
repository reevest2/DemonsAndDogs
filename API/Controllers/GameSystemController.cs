using Microsoft.AspNetCore.Mvc;
using API.Extensions;
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
        var ruleBookResult = registry.Get(systemId);
        if (!ruleBookResult.IsSuccess)
            return ruleBookResult.ToActionResult<Models.Interfaces.IRuleBook, CharacterSheetSchema>();

        return Ok(ruleBookResult.Value!.GetCharacterSheetSchema());
    }
}
