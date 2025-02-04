using CommandService.Dtos;

namespace CommandService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(GenericEventDto eventDto);
}