using Google.Api;
using GrpcService;
using GrpcService.Data;
using GrpcService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcClient<ToDo.ToDoClient>(o =>
{
    o.Address = new Uri("https://localhost:7227");
});
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=ToDoDatabase.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ToDoService>();
app.MapGrpcService<NotificationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
