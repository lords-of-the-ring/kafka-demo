using Microsoft.EntityFrameworkCore;

namespace TodoItems.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(board =>
        {
            board.ToTable("board");
            board.HasKey(x => x.Id);
            board.Property(x => x.Id).ValueGeneratedNever();
            board.Property(x => x.Name).IsUnicode(false).HasMaxLength(20);
        });

        modelBuilder.Entity<TodoItem>(todoItem =>
        {
            todoItem.ToTable("todo_item");
            todoItem.HasKey(x => x.Id);
            todoItem.Property(x => x.Id).ValueGeneratedNever();
            
            todoItem.Property(x => x.Title)
                .IsUnicode(false)
                .HasMaxLength(32);
            
            todoItem.Property(x => x.Description)
                .IsUnicode(false)
                .HasMaxLength(128);
            
            todoItem.HasOne<Board>()
                .WithMany()
                .HasForeignKey(x => x.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
