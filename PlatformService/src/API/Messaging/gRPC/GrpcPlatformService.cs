using AutoMapper;
using Grpc.Core;
using PlatformService.Business.Platform;

namespace PlatformService.API.Messaging.gRPC;

public class GrpcPlatformService(IPlatformHandler handler, IMapper mapper) : GrpcPlatform.GrpcPlatformBase
{
    public override async Task<PlatformResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var platforms = await handler.GetAllAsync();
        return new PlatformResponse
        {
            Platform = { platforms.Select(platform => mapper.Map<GrpcPlatformModel>(platform)) }
        };
    }
}