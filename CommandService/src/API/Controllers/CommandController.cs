using CommandService.Business.Command;
using CommandService.Data.Dto.Command;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.API.Controllers;

[Route("c/[controller]/{platformId}")]
[ApiController]
public class CommandController(ICommandHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetAll(Guid platformId)
    {
        var result = await handler.GetAll(platformId);
        return result.Any() ? Ok(result) : NotFound();
    }
    
    [HttpGet("{commandId}")]
    public async Task<ActionResult<CommandReadDto>> Get(Guid platformId, Guid commandId)
    {
        var result = await handler.Get(platformId, commandId);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody]CommandCreateDto command, Guid platformId)
    {
        var result = await handler.Create(command, platformId);
        return result ? StatusCode(StatusCodes.Status201Created) : StatusCode(StatusCodes.Status409Conflict);
    }
    
    [HttpPut("{commandId}")]
    public async Task<ActionResult> Update([FromBody]CommandUpdateDto command, Guid commandId)
    {
        var result = await handler.Update(command, commandId);
        return result ? NoContent() : NotFound();
    }
    
    [HttpDelete]
    public async Task<ActionResult> Delete(Guid commandId)
    {
        var result = await handler.Delete(commandId);
        return result ? NoContent() : NotFound();
    }
}