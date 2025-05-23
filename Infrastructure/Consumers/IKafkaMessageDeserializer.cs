namespace Infrastructure.Consumers;

public interface IKafkaMessageDeserializer
{
    KafkaMessage Deserialize(string message, Type type);
}
