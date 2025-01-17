using AutoMapper;
using CommandService.Data.Repository.Platform;
using CommandService.Dtos.Platform;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("c/[controller]")]
[ApiController]
public class PlatformController(
    IPlatformRepository repository,
    IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> Get() =>
        Ok(mapper.Map<IEnumerable<PlatformReadDto>>(await repository.GetAllAsync()));
}