using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.Repository.Platform;
using PlatformService.Dtos.Platform;

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
}