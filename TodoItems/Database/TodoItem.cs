namespace TodoItems.Database;

public sealed class TodoItem
{
    private TodoItem() { }
    
    public required Guid Id { get; init; }
    
    public required Guid BoardId { get; init; }
    
    public required string Title { get; init; }
    
    public required string? Description { get; init; }
    
    public required DateTimeOffset DueDate { get; init; }

    public static TodoItem Create(
        Guid id,
        Guid boardId,
        string title,
        string? description,
        DateTimeOffset dueDate)
    {
        return new TodoItem
        {
            Id = id,
            BoardId = boardId,
            Title = title,
            Description = description,
            DueDate = dueDate,
        };
    }
}
