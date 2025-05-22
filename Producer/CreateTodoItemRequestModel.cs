namespace Producer;

public sealed record CreateTodoItemRequestModel
{
    public required string Title { get; init; }

    public required string? Description { get; init; }

    public required DateTime ScheduleDate { get; init; }
}
