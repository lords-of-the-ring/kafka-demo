using Kafka.Abstractions;

namespace Contracts.TodoItems;

public sealed record TodoItemDeleted() : KafkaMessage<TodoItemDeleted>(
    "todo-items",
    "todo-item-deleted",
    message => message.BoardId)
{
    public required Guid TodoItemId { get; init; }
    
    public required Guid BoardId { get; init; }
}
