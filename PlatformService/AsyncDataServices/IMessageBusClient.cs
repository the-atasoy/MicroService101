using PlatformService.Dtos.Platform;

namespace PlatformService.AsyncDataServices;

public interface IMessageBusClient
{
    Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
}