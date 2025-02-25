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
            case EventType.PlatformCreate:
                await CreatePlatform(message);
                break;
            case EventType.PlatformUpdate:
                await UpdatePlatform(message);
                break;
        }
    }

    private async Task CreatePlatform(string platformPublishedMessage)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var handler = serviceScope.ServiceProvider.GetRequiredService<IPlatformHandler>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            await handler.Create(platformPublishedDto!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not create Platform: {ex.Message}");;
            throw;
        }
    }
    
    private async Task UpdatePlatform(string platformPublishedMessage)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var handler = serviceScope.ServiceProvider.GetRequiredService<IPlatformHandler>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            await handler.Update(platformPublishedDto ?? throw new InvalidOperationException("PlatformPublishedDto is null while updating"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not update Platform: {ex.Message}");;
            throw;
        }
    }
    
    private EventType DetermineEvent(string notificaitonMesssage)
    {
        Console.WriteLine("--> Determining Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificaitonMesssage);
        switch (eventType?.Event)
        {
            case "Platform_Create":
                Console.WriteLine("--> Platform Create Event Detected");
                return EventType.PlatformCreate;
            case "Platform_Update":
                Console.WriteLine("--> Platform Update Event Detected");
                return EventType.PlatformUpdate;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }
}

internal enum EventType
{
    PlatformCreate,
    PlatformUpdate,
    Undetermined
}