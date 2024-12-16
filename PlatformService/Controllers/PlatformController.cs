using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.Repository.Platform;
using PlatformService.Dtos.Platform;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("[controller]")]
[ApiController]
public class PlatformController(IPlatformRepository repository, IMapper mapper) : ControllerBase
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
        return StatusCode(StatusCodes.Status201Created);
    }
}