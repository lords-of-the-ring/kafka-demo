using Contracts.TodoItems;
using Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoItems.Database;

namespace TodoItems.Features;

public static class DeleteTodoItem
{
    public sealed record Request(Guid Id);

    public static async Task<IResult> Handle(
        [FromBody] Request request,
        AppDbContext dbContext,
        IKafkaPublisher publisher,
        CancellationToken cancellationToken)
    {
        var todoItem = await dbContext.Set<TodoItem>().SingleAsync(x => x.Id == request.Id, cancellationToken);
        dbContext.Remove(todoItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        var todoItemDeleted = new TodoItemDeleted
        {
            TodoItemId = todoItem.Id,
            BoardId = todoItem.BoardId,
        };

        await publisher.PublishAsync(todoItemDeleted, cancellationToken);

        return Results.NoContent();
    }
}
