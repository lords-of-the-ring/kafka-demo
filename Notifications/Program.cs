using Contracts.TodoItems;
using Kafka;
using Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddKafkaConsumer(
    bootstrapServers: "kafka:9092",
    groupId: "notifications",
    options => options
        .AddHandler<BoardCreated, BoardCreatedHandler>()
        .AddHandler<TodoItemCreated, TodoItemCreatedHandler>()
        .AddHandler<TodoItemDeleted, TodoItemDeletedHandler>());

var app = builder.Build();

app.Run();
