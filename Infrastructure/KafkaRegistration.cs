using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class KafkaRegistration
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaPublisher, KafkaPublisher>();

        services.AddSingleton(_ =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092"
            };

            return new ProducerBuilder<string, byte[]>(config).Build();
        });

        return services;
    }
}
