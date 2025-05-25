using Kafka.Abstractions;

namespace Contracts.TodoItems;

public sealed record TodoItemCreated() : KafkaMessage<TodoItemCreated>(
    "todo-items",
    "todo-item-created",
    message => message.BoardId)
{
    public required Guid TodoItemId { get; init; }

    public required Guid BoardId { get; init; }

    public required string Title { get; init; }

    public required string? Description { get; init; }

    public required DateTimeOffset DueDate { get; init; }
}
