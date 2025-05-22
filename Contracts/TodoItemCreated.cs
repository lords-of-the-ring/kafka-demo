namespace Contracts;

public sealed record TodoItemCreated
{
    public required Guid TodoItemId { get; init; }

    public required Guid BoardId { get; init; }

    public required string Title { get; init; }

    public required string? Description { get; init; }

    public required DateTime ScheduleDate { get; init; }
}
