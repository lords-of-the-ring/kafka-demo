using Contracts;
using Infrastructure;
using Producer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafka();

var app = builder.Build();

app.MapPost("/todo-items",
    async (
        CreateTodoItemRequestModel request,
        IKafkaPublisher publisher,
        CancellationToken cancellationToken) =>
    {
        var johnBoardId = Guid.Parse("e49bfaa8-13a0-49bb-8c29-b92d31034100");
        var steveBoardId = Guid.Parse("3358af6b-aae6-463d-94ca-84850e13b1f2");
        var boardId = Random.Shared.Next() % 2 == 0 ? johnBoardId : steveBoardId;

        var todoItemCreated = new TodoItemCreated
        {
            TodoItemId = Guid.NewGuid(),
            BoardId = boardId,
            Title = request.Title,
            Description = request.Description,
            ScheduleDate = request.ScheduleDate,
        };

        await publisher.PublishAsync(
            KafkaTopics.TodoItems,
            boardId.ToString(),
            todoItemCreated,
            cancellationToken);

        return todoItemCreated;
    });

app.Run();
