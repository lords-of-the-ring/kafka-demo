namespace Infrastructure.Consumers;

public sealed record KafkaConsumerSettings
{
    public required string BootstrapServers { get; init; }

    public required string GroupId { get; init; }

    public required HashSet<string> Topics { get; init; }
}
