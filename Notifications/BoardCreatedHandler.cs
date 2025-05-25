using Contracts.TodoItems;
using Kafka;

namespace Notifications;

public sealed class BoardCreatedHandler(ILogger<BoardCreatedHandler> logger)
    : IKafkaHandler<BoardCreated>
{
    public Task HandleAsync(BoardCreated message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Key - {BoardId} - Board has been created", message.BoardId);
        return Task.CompletedTask;
    }
}
