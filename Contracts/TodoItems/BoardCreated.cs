using Kafka.Abstractions;

namespace Contracts.TodoItems;

public sealed record BoardCreated() : KafkaMessage<BoardCreated>(
    "todo-items",
    "board-created",
    message => message.BoardId)
{
    public required Guid BoardId { get; init; }
    
    public required string Name { get; init; }
}
