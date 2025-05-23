using Contracts;
using Infrastructure.Consumers;

namespace Consumer;

public sealed class TodoItemCreatedHandler : IKafkaMessageHandler<TodoItemCreated>
{
    public Task Handle(TodoItemCreated message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
