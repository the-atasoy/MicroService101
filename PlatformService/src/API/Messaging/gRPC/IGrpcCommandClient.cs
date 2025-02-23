namespace PlatformService.API.Messaging.gRPC;

public interface IGrpcCommandClient
{
    bool DeleteCommand(Guid platformId);
}