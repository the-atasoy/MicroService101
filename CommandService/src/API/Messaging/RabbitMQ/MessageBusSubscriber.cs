using System.Text;
using CommandService.API.Messaging.RabbitMQ.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.API.Messaging.RabbitMQ;

public class MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) : BackgroundService, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        await Initialize();
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel was not initialized properly");
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            Console.WriteLine("--> Event Received!");
            var body = eventArgs.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            eventProcessor.ProcessEvent(message);
            return Task.CompletedTask;
        };
        if (_queueName == null)
            throw new InvalidOperationException("Queue name was not initialized properly");
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
    }

    private async Task Initialize()
    {
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
                _queueName = (await _channel.QueueDeclareAsync()).QueueName;
                await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");
            
                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to Message Bus");
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
    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel was not initialized properly");
        if(_connection == null)
            throw new InvalidOperationException("RabbitMQ connection was not initialized properly");
        if(_channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
        Console.WriteLine("--> MessageBus Disposed");
        base.Dispose();
        await ValueTask.CompletedTask;
    }
}