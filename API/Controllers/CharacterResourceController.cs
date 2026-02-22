using API.Controllers.Abstract;
using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models.Resources.Character;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterResourcesController(ICharacterResourceService service)
    : ResourceControllerBase<CharacterData, ICharacterResourceService>(service)
{
    
}