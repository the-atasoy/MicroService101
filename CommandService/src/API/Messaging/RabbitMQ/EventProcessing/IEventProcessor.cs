namespace CommandService.API.Messaging.RabbitMQ.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}