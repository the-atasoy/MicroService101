using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        InitializeRabbitMq().GetAwaiter().GetResult();
    }

    private async Task InitializeRabbitMq()
    {
        var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQ:HostName"]!, Port = int.Parse(_configuration["RabbitMQ:Port"]!)};
        
        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = (await _channel.QueueDeclareAsync()).QueueName;
            await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");
            Console.WriteLine("--> Connected to Message Bus");
            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
        }
    }
    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
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
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            Console.WriteLine("--> Event Received!");
            var body = eventArgs.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            _eventProcessor.ProcessEvent(message);
            return Task.CompletedTask;
        };
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
    }
}