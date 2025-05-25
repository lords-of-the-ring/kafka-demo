using Kafka.Abstractions;

namespace Kafka;

public interface IKafkaHandler<in T> where T : KafkaMessage<T>
{
    Task HandleAsync(T message, CancellationToken cancellationToken);
}
