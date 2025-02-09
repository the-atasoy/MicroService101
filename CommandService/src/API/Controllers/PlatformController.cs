using CommandService.Business.Platform;
using CommandService.Data.Dto.Platform;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.API.Controllers;

[Route("c/[controller]")]
[ApiController]
public class PlatformController(IPlatformHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> Get() => Ok(await handler.GetAllAsync());
}