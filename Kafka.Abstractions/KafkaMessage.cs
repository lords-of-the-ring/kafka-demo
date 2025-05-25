using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Kafka")]

namespace Kafka.Abstractions;

public abstract record KafkaMessage<TKafkaMessage>
{
    protected KafkaMessage(
        string topicName,
        string typeName,
        Func<TKafkaMessage, object> messagePartitionKey)
    {
        TopicName = topicName;
        TypeName = typeName;
        MessagePartitionKey = messagePartitionKey;
    }

    [JsonIgnore]
    internal Func<TKafkaMessage, object> MessagePartitionKey { get; }
    
    [JsonIgnore]
    internal string TopicName { get; }
    
    [JsonIgnore]
    internal string TypeName { get; }
}
