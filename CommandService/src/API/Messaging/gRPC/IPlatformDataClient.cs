using CommandService.Data.Entity;

namespace CommandService.API.Messaging.gRPC;

public interface IPlatformDataClient
{
    IEnumerable<Platform> ReturnAllPlatforms();
}