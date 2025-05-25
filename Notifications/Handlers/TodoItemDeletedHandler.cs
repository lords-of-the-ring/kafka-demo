using Contracts.TodoItems;
using Kafka;

namespace Notifications.Handlers;

public sealed class TodoItemDeletedHandler(ILogger<TodoItemDeletedHandler> logger)
    : IKafkaHandler<TodoItemDeleted>
{
    public Task HandleAsync(TodoItemDeleted message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Key - {BoardId} - Todo item has been deleted", message.BoardId);
        return Task.CompletedTask;
    }
}
