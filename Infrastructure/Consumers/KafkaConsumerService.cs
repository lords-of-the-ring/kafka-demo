using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Infrastructure.Consumers;

public sealed class KafkaConsumerService(
    ILogger<KafkaConsumerService> _logger,
    KafkaConsumerSettings _kafkaConsumerSettings,
    IKafkaMessageDeserializer _messageDeserializer,
    IMessageHandlerInstanceResolver _handlerInstanceResolver
) : BackgroundService
{
    private IConsumer<string, string>? _consumer;

    private readonly ConcurrentDictionary<string, Channel<ConsumeResult<string, string>>> _keyQueues = [];

    public override Task StartAsync(CancellationToken ct)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaConsumerSettings.BootstrapServers,
            GroupId = _kafkaConsumerSettings.GroupId,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        _consumer.Subscribe(_kafkaConsumerSettings.Topics);

        return base.StartAsync(ct);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(_consumer);

        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = _consumer.Consume(stoppingToken);

                var key = message.Message.Key ?? "__null__";

                var candidate = Channel.CreateUnbounded<ConsumeResult<string, string>>();

                var channel = _keyQueues.GetOrAdd(key, candidate);

                if (channel == candidate)
                {
                    _ = RunKeyProcessor(key, candidate.Reader, stoppingToken);
                }

                await channel.Writer.WriteAsync(message, stoppingToken);
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }

    private async Task RunKeyProcessor(
        string key,
        ChannelReader<ConsumeResult<string, string>> reader,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_consumer);

        await foreach (var msg in reader.ReadAllAsync(ct))
        {
            try
            {
                Type type = null!;
                var message = _messageDeserializer.Deserialize(msg.Message.Value, type);
                var handler = _handlerInstanceResolver.ResolveHandler(message);
                await ((dynamic)handler).Handle((dynamic)message, ct);
                _consumer.Commit(msg);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing key {Key}", key);
            }
        }

        // optionally remove the channel when done
        _keyQueues.TryRemove(key, out _);
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }

    // your existing async handler
    //private async Task ProcessTodoItemAsync(TodoItemCreated item, CancellationToken ct)
    //{
    //    if (item.BoardId == Guid.Parse("e49bfaa8-13a0-49bb-8c29-b92d31034100"))
    //    {
    //        await Task.Delay(15_000, ct);
    //        _logger.LogInformation("A) 15 sec => Process item with ID: '{Id}'", item.TodoItemId);
    //    }
    //    else
    //    {
    //        await Task.Delay(5_000, ct);
    //        _logger.LogInformation("B) 5 sec => Process item with ID: '{Id}'", item.TodoItemId);
    //    }
    //}
}
