using Microsoft.AspNetCore.Mvc;
using PlatformService.Business.Platform;
using PlatformService.Data.Dto.Platform;

namespace PlatformService.API.Controllers;

[Route("[controller]")]
[ApiController]
public class PlatformController(IPlatformHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> Get() => Ok(await handler.GetAllAsync());
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlatformReadDto>> GetById(Guid id)
    {
        var result = await handler.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(PlatformCreateDto platform)
    {
        var result = await handler.CreateAsync(platform);
        return result ? StatusCode(StatusCodes.Status201Created) : NotFound();
    }
}