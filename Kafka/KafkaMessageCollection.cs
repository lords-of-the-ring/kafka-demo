using Kafka.Abstractions;

namespace Kafka;

public sealed class KafkaMessageCollection
{
    private readonly Dictionary<Key, Value> _messages = [];
    
    private bool _initialized = false;
    
    internal KafkaMessageCollection() { }

    internal void Initialize() => _initialized = true;
    
    internal Type GetMessageType(string topic, string typeName)
    {
        var key = new Key(topic, typeName);
        return _messages[key].MessageType;
    }
    
    internal Type GetHandlerType(string topic, string typeName)
    {
        var key = new Key(topic, typeName);
        return _messages[key].HandlerType;
    }

    internal IEnumerable<string> GetAllTopics() => _messages.Keys.Select(k => k.Topic).Distinct();
    
    internal IEnumerable<Type> GetAllHandlerTypes() => _messages.Values.Select(v => v.HandlerType);
    
    public KafkaMessageCollection AddHandler<TMessage, THandler>()
        where TMessage : KafkaMessage<TMessage>
        where THandler : IKafkaHandler<TMessage>
    {
        if (_initialized)
        {
            throw new InvalidOperationException("Kafka message collection is already initialized");    
        }
        
        var message = Activator.CreateInstance<TMessage>();
        var key = new Key(message.TopicName, message.TypeName);
        var value = new Value(typeof(TMessage), typeof(THandler));
        _messages.Add(key, value);
        
        return this;
    }
    
    private sealed record Key(string Topic, string TypeName);
    
    private sealed record Value(Type MessageType, Type HandlerType);
}
