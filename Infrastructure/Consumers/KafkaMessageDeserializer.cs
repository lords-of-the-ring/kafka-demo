using System.Text.Json;

namespace Infrastructure.Consumers;

internal sealed class KafkaMessageDeserializer : IKafkaMessageDeserializer
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public KafkaMessage Deserialize(string message, Type type)
    {
        var kafkaMessage = JsonSerializer.Deserialize(message, type, _jsonOptions)!;

        return kafkaMessage as KafkaMessage ?? throw new InvalidOperationException("Unable to cast object to kafka message");
    }
}
