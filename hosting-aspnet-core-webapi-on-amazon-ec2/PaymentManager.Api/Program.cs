using Microsoft.EntityFrameworkCore;
using PaymentManager.Api;
using PaymentManager.Api.DTOs;
using PaymentManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddHostedService<ConsumerOrderMessageService>();
builder.Services.AddScoped<IUpdateOrderMessageRepository, UpdateOrderMessageRepository>();

var assemblyName = typeof(Program).Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    c => c.UseNpgsql(connectionString, m => m.MigrationsAssembly(assemblyName)));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Minimal API to create a new order
app.MapPost("/payments", async (IPaymentService paymentService, ProcessPaymentRequest request) =>
{
    var orderId = await paymentService.ProcessPaymentAsync(request);

    return Results.Created($"/payments/{orderId}", request);
});

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    await context.Database.MigrateAsync();
}

app.Run();
