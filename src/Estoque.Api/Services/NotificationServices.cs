using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Estoque.Api.Services;

public class NotificationBroadcastService
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _channels = [];

    public (Guid id, ChannelReader<string> reader) Subscribe()
    {
        var id = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>();
        _channels.TryAdd(id, channel);
        return (id, channel.Reader);
    }

    public void Unsubscribe(Guid id)
    {
        if (_channels.TryRemove(id, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }

    public async Task BroadcastAsync(string payload)
    {
        foreach (var entry in _channels.Values)
        {
            await entry.Writer.WriteAsync(payload);
        }
    }
}

public class RabbitMqConsumerBackgroundService(
    IConfiguration configuration,
    ILogger<RabbitMqConsumerBackgroundService> logger,
    NotificationBroadcastService notifications) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = configuration["RabbitMq:QueueName"] ?? "stock-events";
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = await factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
                await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, eventArgs) =>
                {
                    var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                    await notifications.BroadcastAsync(message);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                };

                await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha ao consumir eventos RabbitMQ. Nova tentativa em 5 segundos.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
