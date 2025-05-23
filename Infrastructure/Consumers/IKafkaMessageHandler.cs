namespace Infrastructure.Consumers;

public interface IKafkaMessageHandler<T> where T : KafkaMessage
{
    Task Handle(T message, CancellationToken cancellationToken);
}
