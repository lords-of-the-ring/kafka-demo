using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;

namespace Kafka;

internal sealed class TopicFactory(
    KafkaConsumerSettings settings,
    ILogger<TopicFactory> logger
) : ITopicFactory
{
    public async Task CreateTopicAsync(string topic)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = settings.BootstrapServers
        };

        using var admin = new AdminClientBuilder(adminConfig).Build();

        var topicSpec = new TopicSpecification
        {
            Name = topic,
            NumPartitions = 3,
            ReplicationFactor = 1,
            Configs = new Dictionary<string, string>
            {
                ["retention.ms"] = "172800000"
            }
        };

        try
        {
            await admin.CreateTopicsAsync([topicSpec]);
        }
        catch (CreateTopicsException e)
        {
            if (e.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                logger.LogInformation("Topic '{Topic}' already exists.", topic);
                return;
            }

            foreach (var result in e.Results)
            {
                if (result.Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    logger.LogError("Error creating {Topic}: {Reason}",
                        result.Topic,
                        result.Error.Reason);
                }
            }

            throw;
        }
    }
}
