namespace Kafka;

internal sealed record KafkaConsumerSettings
{
    public required string BootstrapServers { get; init; }

    public required string GroupId { get; init; }
}
