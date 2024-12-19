using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return Ok("Inbound test from Platforms Controller");
    }
}