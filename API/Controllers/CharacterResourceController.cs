using API.Controllers.Abstract;
using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models.Resources;

[Route("api/[controller]")]
public class CharacterResourcesController(ICharacterResourceService service)
    : ResourceControllerBase<CharacterResource, ICharacterResourceService>(service)
{
    
}