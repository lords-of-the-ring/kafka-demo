namespace TodoItems.Database;

public sealed class Board
{
    private Board() { }
    
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }

    public static Board Create(Guid id, string name) => new() { Id = id, Name = name };
}
