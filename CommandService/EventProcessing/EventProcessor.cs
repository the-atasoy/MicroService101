using System.Text.Json;
using AutoMapper;
using CommandService.Data.Repository.Platform;
using CommandService.Dtos;
using CommandService.Dtos.Platform;

namespace CommandService.EventProcessing;

public class EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper) : IEventProcessor
{
    public async Task ProcessEvent(GenericEventDto eventDto)
    {
        var eventType = DetermineEvent(eventDto.Event);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                // TODO: Add Platform
                break;
            default:
                break;
        }
    }

    private async Task addPlatform(string platformPublishedMessage)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var repo = serviceScope.ServiceProvider.GetRequiredService<IPlatformRepository>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            var platform = mapper.Map<Models.Platform>(platformPublishedDto);
            if (!repo.IsExternalPlatformExistAsync(platform.ExternalId).Result)
            {
                await repo.CreateAsync(platform);
                await repo.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("--> Platform already exists in the DB");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not add Platform to DB: {ex.Message}");;
            throw;
        }
    }
    
    private EventType DetermineEvent(string notificaitonMesssage)
    {
        Console.WriteLine("--> Determining Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificaitonMesssage);
        switch (eventType?.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform Published Event Detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}