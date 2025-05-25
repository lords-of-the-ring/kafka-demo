using Contracts.TodoItems;
using Kafka;

namespace Notifications;

public sealed class TodoItemCreatedHandler(ILogger<TodoItemCreatedHandler> logger)
    : IKafkaHandler<TodoItemCreated>
{
    public Task HandleAsync(TodoItemCreated message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Key - {BoardId} - Todo item has been created", message.BoardId);
        return Task.CompletedTask;
    }
}
