using API.Controllers.Abstract;
using API.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Models.Resources.Character;

namespace API.Controllers;

[ApiController]
[Route("api/charactertemplateresources")]
public class CharacterTemplateResourcesController(ICharacterTemplateResourceService service)
    : ResourceControllerBase<CharacterTemplateData, ICharacterTemplateResourceService>(service)
{
    
}