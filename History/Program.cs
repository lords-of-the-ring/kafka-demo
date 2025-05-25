using Contracts.TodoItems;
using History.Handlers;
using Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddKafkaConsumer(
    bootstrapServers: "kafka:9092",
    groupId: "history",
    options => options
        .AddHandler<BoardCreated, BoardCreatedHandler>()
        .AddHandler<TodoItemCreated, TodoItemCreatedHandler>()
        .AddHandler<TodoItemDeleted, TodoItemDeletedHandler>());

var app = builder.Build();

app.Run();
