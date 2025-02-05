using AutoMapper;
using CommandService.Data.Repository.Command;
using CommandService.Data.Repository.Platform;
using CommandService.Dtos.Command;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("c/[controller]/{platformId}")]
[ApiController]
public class CommandController(
    ICommandRepository repository,
    IPlatformRepository platformRepository,
    IMapper mapper)
    : ControllerBase
{
    [HttpGet("{commandId}")]
    public async Task<ActionResult<CommandReadDto>> Get(Guid platformId, Guid commandId)
    {
        var command = await repository.GetAsync(platformId, commandId);
        return command is null ? NotFound() : Ok(mapper.Map<CommandReadDto>(command));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetAll(Guid platformId)
    {
        if(!await platformRepository.IsPlatformExistAsync(platformId))
            return NotFound();
        
        return Ok(mapper.Map<List<CommandReadDto>>(await repository.GetAllAsync(platformId)));
    }
    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody]CommandCreateDto commandCreateDto, Guid platformId)
    {
        if(!await platformRepository.IsPlatformExistAsync(platformId))
            return NotFound();
        
        var command = mapper.Map<Models.Command>(commandCreateDto);
        command.PlatformId = platformId;
        await repository.CreateAsync(command);
        await repository.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created);
    }
}