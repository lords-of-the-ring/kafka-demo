namespace Kafka;

internal interface ITopicFactory
{
    Task CreateTopicAsync(string topic);
}
