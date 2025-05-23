using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Consumers;

public static class KafkaRegistration
{
    public static IServiceCollection AddKafkaHostedServiceConsumer(this IServiceCollection services,
        KafkaConsumerSettings settings)
    {
        services.AddHostedService<KafkaConsumerService>();

        services.AddSingleton(settings);

        return services;
    }
}
