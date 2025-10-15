using Microsoft.EntityFrameworkCore;
using OrderManager.Api;
using OrderManager.Api.DTOs;
using OrderManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddHostedService<ConsumerUpdateOrderMessageService>();
builder.Services.AddScoped<IOrderMessageRepository, OrderMessageRepository>();


var assemblyName = typeof(Program).Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    c => c.UseNpgsql(connectionString, m => m.MigrationsAssembly(assemblyName)));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Minimal API to get all orders
app.MapGet("/orders", async (IOrderService orderService) =>
{
    var orders = await orderService.GetOrderAsync();
    return Results.Ok(orders);
});

// Minimal API to get a order by ID
app.MapGet("/orders/{id:guid}", async (IOrderService orderService, Guid id) =>
{
    var order = await orderService.GetOrderAsync(id);
    if (order == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(order);
});

// Minimal API to create a new order
app.MapPost("/orders", async (IOrderService orderService, IOrderMessageRepository orderMessageRepository, CreateOrderRequest request) =>
{
    var orderId = await orderService.CreateOrderAsync(request);

    await orderMessageRepository.SendOrderMessage(new Core.OrderMessage
    {
        OrderId = orderId,
        Description = request.Description,
        Price = request.Price,
        CreatedAt = DateTime.UtcNow
    });

    return Results.Created($"/orders/{orderId}", request);
});

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    await context.Database.MigrateAsync();
}

app.Run();