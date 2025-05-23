using Infrastructure.Consumers;

namespace Consumer;

public sealed record TestKafkaMessage : KafkaMessage
{
    public required string Name { get; init; }
}
