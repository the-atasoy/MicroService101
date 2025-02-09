using PlatformService.Data.Dto.Platform;

namespace PlatformService.API.Messaging.RabbitMQ;

public interface IMessageBusClient
{
    Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
}