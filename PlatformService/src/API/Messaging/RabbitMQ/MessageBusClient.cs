using System.Text;
using System.Text.Json;
using PlatformService.Data.Dto.Platform;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.API.Messaging.RabbitMQ;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConnection _connection = null!;
    private readonly IChannel _channel = null!;

    public MessageBusClient(IConfiguration configuration)
    {
        var factory = new ConnectionFactory() {HostName = configuration["RabbitMQ:HostName"]!, Port = int.Parse(configuration["RabbitMQ:Port"]!)};
        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
            Console.WriteLine("--> Connected to Message Bus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
        }
    }
    
    public async Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);
        if(_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
        }
    }
    
    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(
            exchange: "trigger",
            routingKey: "",
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: body);
        Console.WriteLine($"--> We have sent {message}");
    }
    
    public async Task Dispose()
    {
        Console.WriteLine("--> MessageBus Disposed");
        if(_channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
    
    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
    }
}