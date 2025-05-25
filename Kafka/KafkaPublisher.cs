using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Kafka.Abstractions;

namespace Kafka;

internal sealed class KafkaPublisher(IProducer<string, byte[]> producer) : IKafkaPublisher
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        where T : KafkaMessage<T>
    {
        var partitionKey = message.MessagePartitionKey(message).ToString();
        ArgumentNullException.ThrowIfNull(partitionKey);
        
        var value = JsonSerializer.SerializeToUtf8Bytes(message);
        var typeName = Encoding.UTF8.GetBytes(message.TypeName);
        
        var kafkaMessage = new Message<string, byte[]>
        {
            Key = partitionKey,
            Value = value,
            Headers = [ new Header(Constants.TypeNameHeader, typeName) ]
        };

        await producer.ProduceAsync(message.TopicName, kafkaMessage, cancellationToken);
    }
}
