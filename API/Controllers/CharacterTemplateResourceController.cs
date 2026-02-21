using API.Controllers.Abstract;
using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class CharacterTemplateResourcesController(ICharacterTemplateResourceService service)
    : ResourceControllerBase<CharacterTemplateData, ICharacterTemplateResourceService>(service)
{
    
}