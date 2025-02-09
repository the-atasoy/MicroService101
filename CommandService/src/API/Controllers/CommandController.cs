using CommandService.Business.Command;
using CommandService.Data.Dto.Command;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.API.Controllers;

[Route("c/[controller]/{platformId}")]
[ApiController]
public class CommandController(ICommandHandler handler) : ControllerBase
{
    [HttpGet("{commandId}")]
    public async Task<ActionResult<CommandReadDto>> Get(Guid platformId, Guid commandId)
    {
        var result = await handler.GetAsync(platformId, commandId);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetAll(Guid platformId)
    {
        var result = await handler.GetAllAsync(platformId);
        return result.Any() ? Ok(result) : NotFound();
    }
    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody]CommandCreateDto command, Guid platformId)
    {
        var result = await handler.CreateAsync(command, platformId);
        return result ? StatusCode(StatusCodes.Status201Created) : NotFound();
    }
}