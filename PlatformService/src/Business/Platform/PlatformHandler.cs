using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlatformService.API.Messaging.gRPC;
using PlatformService.API.Messaging.RabbitMQ;
using PlatformService.Data;
using PlatformService.Data.Dto.Platform;

namespace PlatformService.Business.Platform;

public class PlatformHandler(
    AppDbContext context,
    IMapper mapper,
    IMessageBusClient messageBusClient,
    IGrpcCommandClient grpcCommandClient) : IPlatformHandler
{
    public async Task<IEnumerable<PlatformReadDto>> GetAll() =>
        mapper.Map<IEnumerable<PlatformReadDto>>(await context.Platform.AsNoTracking().ToListAsync());
    
    public async Task<PlatformReadDto?> Get(Guid id) =>
        mapper.Map<PlatformReadDto>(await context.Platform.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id));

    public async Task<bool> Create(PlatformCreateDto platform)
    {
        if (await context.Platform.Where(p => p.Name == platform.Name).AnyAsync()) return false;
        
        var entity = mapper.Map<Data.Entity.Platform>(platform);
        await context.Platform.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)));
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(entity);
            platformPublishedDto.Event = "Platform_Create";
            await messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
            return false;
        }
        return await context.SaveChangesAsync() >= 0;
    }
    
    public async Task<bool> Update(Guid id, PlatformUpdateDto platform)
    {
        var entity = await context.Platform.FindAsync(id);
        if (entity == null) return false;
        
        mapper.Map(platform, entity);
        context.Platform.Update(entity);
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(entity);
            platformPublishedDto.Event = "Platform_Update";
            await messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
            return false;
        }
        return await context.SaveChangesAsync() >= 0;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await context.Platform.FindAsync(id);
        context.Platform.Remove(entity ?? throw new ArgumentNullException(nameof(entity)));
        
        var grpcResponse = grpcCommandClient.DeleteCommand(id);
        if (!grpcResponse) return false;
        
        return await context.SaveChangesAsync() >= 0;
    }
}