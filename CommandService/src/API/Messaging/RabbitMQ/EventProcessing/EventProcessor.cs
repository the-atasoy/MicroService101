using System.Text.Json;
using CommandService.Business.Platform;
using CommandService.Data.Dto;
using CommandService.Data.Dto.Platform;

namespace CommandService.API.Messaging.RabbitMQ.EventProcessing;

public class EventProcessor(IServiceScopeFactory serviceScopeFactory) : IEventProcessor
{
    public async Task ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                await AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private async Task AddPlatform(string platformPublishedMessage)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var handler = serviceScope.ServiceProvider.GetRequiredService<IPlatformHandler>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            await handler.CreateAsync(platformPublishedDto!);
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

internal enum EventType
{
    PlatformPublished,
    Undetermined
}