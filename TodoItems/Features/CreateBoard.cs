using Contracts.TodoItems;
using Kafka;
using TodoItems.Database;

namespace TodoItems.Features;

public static class CreateBoard
{
    public sealed record Request(Guid Id, string Name);

    public static async Task<IResult> Handle(
        Request request,
        AppDbContext dbContext,
        IKafkaPublisher publisher,
        CancellationToken cancellationToken)
    {
        var board = Board.Create(request.Id, request.Name);
        dbContext.Set<Board>().Add(board);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        var boardCreated = new BoardCreated { BoardId = board.Id, Name = board.Name };
        await publisher.PublishAsync(boardCreated, cancellationToken);
        
        return Results.Ok(new { BoardId = board.Id, board.Name });
    }
}
