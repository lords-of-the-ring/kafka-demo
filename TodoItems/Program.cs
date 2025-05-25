using Kafka;
using Microsoft.EntityFrameworkCore;
using TodoItems.Database;
using TodoItems.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaProducer("kafka:9092");

const string connectionString = "Host=postgres;Port=5432;Username=postgres;Password=Qwerty1@;Database=todo-items;";
builder.Services.AddDbContext<AppDbContext>(x => x.UseNpgsql(connectionString));

var app = builder.Build();

using var scope = app.Services.CreateScope();
using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.EnsureCreated();

app.MapPost("/boards", CreateBoard.Handle);

app.MapPost("/todo-items", CreateTodoItem.Handle);

app.MapDelete("/todo-items", DeleteTodoItem.Handle);

app.Run();
