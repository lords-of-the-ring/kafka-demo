using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Kafka;

internal sealed class KafkaBackgroundService : BackgroundService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IConsumer<string, string> _consumer;
    private readonly KafkaMessageCollection _messageCollection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<KafkaBackgroundService> _logger;
    private readonly ITopicFactory _topicFactory;
    private readonly ConcurrentDictionary<string, Channel<ConsumeResult<string, string>>> _queues = [];

    public KafkaBackgroundService(
        KafkaConsumerSettings settings,
        KafkaMessageCollection messageCollection,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<KafkaBackgroundService> logger,
        ITopicFactory topicFactory)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            GroupId = settings.GroupId,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        _messageCollection = messageCollection;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _topicFactory = topicFactory;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var topics = _messageCollection.GetAllTopics().ToList();

        foreach (var topic in topics)
        {
            await _topicFactory.CreateTopicAsync(topic);
        }

        _consumer.Subscribe(topics);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var partitionKey = consumeResult.Message.Key ?? Constants.DefaultPartitionKey;
                var candidate = Channel.CreateUnbounded<ConsumeResult<string, string>>();
                var channel = _queues.GetOrAdd(partitionKey, candidate);

                if (channel == candidate)
                {
                    _ = RunKeyProcessor(partitionKey, candidate.Reader, stoppingToken);
                }

                await channel.Writer.WriteAsync(consumeResult, stoppingToken);
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }

    private async Task RunKeyProcessor(
        string partitionKey,
        ChannelReader<ConsumeResult<string, string>> reader,
        CancellationToken cancellationToken)
    {
        await foreach (var consumeResult in reader.ReadAllAsync(cancellationToken))
        {
            var processed = false;

            while (!processed)
            {
                try
                {
                    var typeNameHeader = consumeResult.Message.Headers.Single(h => h.Key == Constants.TypeNameHeader);
                    var typeName = Encoding.UTF8.GetString(typeNameHeader.GetValueBytes());

                    var messageType = _messageCollection.GetMessageType(consumeResult.Topic, typeName);
                    var message = JsonSerializer.Deserialize(
                        consumeResult.Message.Value,
                        messageType,
                        JsonSerializerOptions);

                    ArgumentNullException.ThrowIfNull(message);

                    var handlerType = _messageCollection.GetHandlerType(consumeResult.Topic, typeName);

                    await using var scope = _serviceScopeFactory.CreateAsyncScope();
                    var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                    await ((dynamic)handler).HandleAsync((dynamic)message, cancellationToken);

                    _consumer.Commit(consumeResult);
                    processed = true;
                }
                catch (Exception e)
                {
                    processed = false;
                    _logger.LogError(e, "Error while processing message with key: '{Key}'.", partitionKey);
                    await Task.Delay(5_000, cancellationToken);
                }
            }
        }

        _queues.TryRemove(partitionKey, out _);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
