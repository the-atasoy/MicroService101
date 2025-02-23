using Grpc.Net.Client;

namespace PlatformService.API.Messaging.gRPC;

public class GrpcCommandClient(IConfiguration configuration) :  IGrpcCommandClient
{
   public bool DeleteCommand(Guid platformId)
    {
        var channel = GrpcChannel.ForAddress(configuration["GrpcCommand"]!);
        var client = new GrpcCommand.GrpcCommandClient(channel);
        var request = new DeleteCommandRequest() { PlatformId = platformId.ToString() };
        try
        {
            var response = client.DeleteCommand(request);
            return response.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call the gRPC Server. {ex.Message}");
            return false;
        }
    }
}