using AutoMapper;
using Grpc.Core;
using PlatformService.Business.Platform;

namespace PlatformService.API.Messaging.gRPC;

public class GrpcPlatformService(IPlatformHandler handler, IMapper mapper) : GrpcPlatform.GrpcPlatformBase
{
   public override Task<DeletePlatformResponse> DeleteCommand(DeletePlatformRequest request, ServerCallContext context)
    {
        Console.WriteLine($"--> Received Grpc Request to delete commands for Platform: {request.PlatformId}");
        return Task.FromResult(new DeletePlatformResponse
        {
            Success = true
        });
    }
}