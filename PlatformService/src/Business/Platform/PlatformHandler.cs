using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlatformService.API.Messaging.RabbitMQ;
using PlatformService.Data;
using PlatformService.Data.Dto.Platform;

namespace PlatformService.Business.Platform;

public class PlatformHandler(AppDbContext context, IMapper mapper, IMessageBusClient messageBusClient) : IPlatformHandler
{
    public async Task<IEnumerable<PlatformReadDto>> GetAllAsync() =>
        mapper.Map<IEnumerable<PlatformReadDto>>(await context.Platform.ToListAsync());
    
    public async Task<PlatformReadDto?> GetByIdAsync(Guid id) =>
        mapper.Map<PlatformReadDto>(await context.Platform.FirstOrDefaultAsync(p => p.Id == id));

    public async Task<bool> CreateAsync(PlatformCreateDto platform)
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
}