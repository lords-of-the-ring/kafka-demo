using System.Text.Json.Serialization;

namespace Infrastructure.Consumers;

public abstract record KafkaMessage
{
    [JsonInclude]
    [JsonPropertyName("$type")]
    internal string KafkaType => GetType().Name;
}
