using Kafka.Abstractions;

namespace Kafka;

public interface IKafkaPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        where T : KafkaMessage<T>;
}
