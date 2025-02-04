using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data.Repository.Platform;
using PlatformService.Dtos.Platform;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("[controller]")]
[ApiController]
public class PlatformController(
    IPlatformRepository repository,
    IMapper mapper,
    ICommandDataClient commandDataClient,
    IMessageBusClient messageBusClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetAll()
    {
        var platforms = await repository.GetAllAsync();
        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlatformReadDto>> GetById(Guid id)
    {
        var platform = await repository.GetByIdAsync(id);
        return platform == null ? NotFound() : Ok(mapper.Map<PlatformReadDto>(platform));
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(PlatformCreateDto platformCreateDto)
    {
        await repository.CreateAsync(mapper.Map<Platform>(platformCreateDto));
        await repository.SaveChangesAsync();
        
        // Send Sync Message
        try
        {
            await commandDataClient.SendPlatformToCommand(mapper.Map<PlatformReadDto>(platformCreateDto));
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send synchronously: {e.Message}");
        }
        
        // Send Async Message
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformCreateDto);
            platformPublishedDto.Event = "Platform_Published";
            await messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
        }
        
        return StatusCode(StatusCodes.Status201Created);
    }
}