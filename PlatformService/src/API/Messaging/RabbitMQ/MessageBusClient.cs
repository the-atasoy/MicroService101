using System.Text;
using System.Text.Json;
using PlatformService.Data.Dto.Platform;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.API.Messaging.RabbitMQ;

public class MessageBusClient(IConfiguration configuration) : IMessageBusClient
{
    private IConnection _connection = null!;
    private IChannel _channel = null!;
    private bool _initialized;
    
    private async Task Initialize()
    {
        if (_initialized) return;

        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:HostName"]!,
            Port = int.Parse(configuration["RabbitMQ:Port"]!)
        };

        const int maxRetries = 10;
        const int delaySeconds = 5;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                Console.WriteLine($"--> Attempting to connect to Message Bus (Attempt {i + 1}/{maxRetries})");
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to Message Bus");
                _initialized = true;
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");

                if (i == maxRetries - 1)
                {
                    Console.WriteLine("--> Connection failed after all retry attempts");
                    throw;
                }

                Console.WriteLine($"--> Retrying in {delaySeconds} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }
    }
    
    public async Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        await Initialize();
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