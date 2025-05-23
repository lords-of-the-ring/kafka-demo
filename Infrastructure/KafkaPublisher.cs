using Confluent.Kafka;
using System.Text.Json;

namespace Infrastructure;

internal sealed class KafkaPublisher(IProducer<string, byte[]> _producer)
    : IKafkaPublisher
{
    public async Task PublishAsync<TMessageValue>(
        string topic,
        string partitionKey,
        TMessageValue value,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(value);

        var message = new Message<string, byte[]>
        {
            Key = partitionKey,
            Value = payload,
        };

        await _producer.ProduceAsync(topic, message, cancellationToken);
    }
}
