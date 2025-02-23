using CommandService.Business.Command;
using Grpc.Core;

namespace CommandService.API.Messaging.gRPC;

public class GrpcCommandService(ICommandHandler handler) : GrpcCommand.GrpcCommandBase
{
    public override async Task<DeleteCommandResponse> DeleteCommand(DeleteCommandRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.PlatformId, out var platformId))
            {
                return new DeleteCommandResponse { Success = false };
            }
            var isDeleted = await handler.DeleteAll(platformId);
            return isDeleted ? new DeleteCommandResponse { Success = true } : new DeleteCommandResponse { Success = false };
        }
        catch
        {
            return new DeleteCommandResponse { Success = false };
        }
    }
}