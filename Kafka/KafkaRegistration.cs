using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka;

public static class KafkaRegistration
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services,
        string bootstrapServers)
    {
        services.AddSingleton<IKafkaPublisher, KafkaPublisher>();

        services.AddSingleton(_ =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            return new ProducerBuilder<string, byte[]>(config).Build();
        });

        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services,
        string bootstrapServers,
        string groupId,
        Action<KafkaMessageCollection> configureMessageCollection)
    {
        var kafkaMessageCollection = new KafkaMessageCollection();
        configureMessageCollection(kafkaMessageCollection);
        kafkaMessageCollection.Initialize();

        foreach (var handlerType in kafkaMessageCollection.GetAllHandlerTypes())
        {
            services.AddScoped(handlerType);
        }

        services.AddSingleton(kafkaMessageCollection);

        services.AddHostedService<KafkaBackgroundService>();

        services.AddSingleton<ITopicFactory, TopicFactory>();

        services.AddSingleton(new KafkaConsumerSettings
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
        });

        return services;
    }
}
