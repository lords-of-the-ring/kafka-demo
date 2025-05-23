namespace Infrastructure.Consumers;

public interface IMessageHandlerInstanceResolver
{
    object ResolveHandler(KafkaMessage kafkaMessage);
}
