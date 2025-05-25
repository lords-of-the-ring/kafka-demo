using Contracts.TodoItems;
using Kafka;
using TodoItems.Database;

namespace TodoItems.Features;

public static class CreateTodoItem
{
    public sealed record Request(
        Guid Id,
        Guid BoardId,
        string Title,
        string? Description,
        DateTimeOffset DueDate);

    public static async Task<IResult> Handle(
        Request request,
        AppDbContext dbContext,
        IKafkaPublisher publisher,
        CancellationToken cancellationToken)
    {
        var todoItem = TodoItem.Create(
            request.Id,
            request.BoardId,
            request.Title,
            request.Description,
            request.DueDate);
        
        dbContext.Set<TodoItem>().Add(todoItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        var todoItemCreated = new TodoItemCreated
        {
            TodoItemId = todoItem.Id,
            BoardId = todoItem.BoardId,
            Title = todoItem.Title,
            Description = todoItem.Description,
            DueDate = todoItem.DueDate,
        };
        
        await publisher.PublishAsync(todoItemCreated, cancellationToken);
        
        return Results.Ok(new
        {
            TodoItemId = todoItem.Id,
            todoItem.Title
        });
    }
}
