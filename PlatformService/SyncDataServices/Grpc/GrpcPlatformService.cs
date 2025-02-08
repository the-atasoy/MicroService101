using AutoMapper;
using Grpc.Core;
using PlatformService.Data.Repository.Platform;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService(IPlatformRepository repository, IMapper mapper) : GrpcPlatform.GrpcPlatformBase
{
    public override async Task<PlatformResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var platforms = await repository.GetAllAsync();
        return new PlatformResponse
        {
            Platform = { platforms.Select(platform => mapper.Map<GrpcPlatformModel>(platform)) }
        };
    }
}