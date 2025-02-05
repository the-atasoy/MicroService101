using CommandService.Dtos;

namespace CommandService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}