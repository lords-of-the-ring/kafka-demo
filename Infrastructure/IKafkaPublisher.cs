namespace Infrastructure;

public interface IKafkaPublisher
{
    Task PublishAsync<TMessageValue>(
        string topic,
        string partitionKey,
        TMessageValue value,
        CancellationToken cancellationToken);
}
