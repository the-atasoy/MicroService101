using Microsoft.AspNetCore.Mvc;
using PlatformService.Business.Platform;
using PlatformService.Data.Dto.Platform;

namespace PlatformService.API.Controllers;

[Route("[controller]")]
[ApiController]
public class PlatformController(IPlatformHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetAll() => Ok(await handler.GetAll());
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlatformReadDto>> Get(Guid id)
    {
        var result = await handler.Get(id);
        return result == null ? NotFound() : Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(PlatformCreateDto platform)
    {
        var result = await handler.Create(platform);
        return result ? StatusCode(StatusCodes.Status201Created) : StatusCode(StatusCodes.Status409Conflict);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await handler.Delete(id);
        return result ? Ok() : NotFound();
    }
}