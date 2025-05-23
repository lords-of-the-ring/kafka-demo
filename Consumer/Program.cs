using Consumer;
using Contracts;
using Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaHostedServiceConsumer(new KafkaConsumerSettings
{
    BootstrapServers = "kafka:9092",
    GroupId = "manny",
    Topics = [KafkaTopics.TodoItems],
});

builder.Services.AddLogging();

var app = builder.Build();

app.MapGet("/", () =>
{
    var message = new TestKafkaMessage
    { Name = "test" };

    return message;
});

app.Run();
