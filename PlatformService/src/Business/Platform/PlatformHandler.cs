using AutoMapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using PlatformService.API.Messaging.RabbitMQ;
using PlatformService.Data;
using PlatformService.Data.Dto.Platform;

namespace PlatformService.Business.Platform;

public class PlatformHandler(
    AppDbContext context,
    IMapper mapper,
    IMessageBusClient messageBusClient,
    IConfiguration configuration) : IPlatformHandler
{
    public async Task<IEnumerable<PlatformReadDto>> GetAll() =>
        mapper.Map<IEnumerable<PlatformReadDto>>(await context.Platform.ToListAsync());
    
    public async Task<PlatformReadDto?> Get(Guid id) =>
        mapper.Map<PlatformReadDto>(await context.Platform.FirstOrDefaultAsync(p => p.Id == id));

    public async Task<bool> Create(PlatformCreateDto platform)
    {
        var entity = mapper.Map<Data.Entity.Platform>(platform);
        await context.Platform.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)));
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platform);
            platformPublishedDto.Event = "Platform_Published";
            await messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
            return false;
        }
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await context.Platform.FindAsync(id);
        context.Platform.Remove(entity ?? throw new ArgumentNullException(nameof(entity)));
        try
        {
            var channel = GrpcChannel.ForAddress("http://platforms-clusterip-srv:666");
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new DeletePlatformRequest { PlatformId = id.ToString() };
            client.DeleteCommand(request);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        var result = await context.SaveChangesAsync();
        return result >= 0;
    }
}