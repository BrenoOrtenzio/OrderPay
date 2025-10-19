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
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var assemblyName = typeof(Program).Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    c => c.UseNpgsql(connectionString, m => m.MigrationsAssembly(assemblyName)));

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/payments", async (IPaymentService paymentService) =>
{
    var payments = await paymentService.GetPaymentsAsync();
    return Results.Ok(payments);
});

app.MapGet("/orders/{id:guid}", async (IPaymentService paymentService, Guid id) =>
{
    var payment = await paymentService.GetPaymentAsync(id);
    if (payment == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(payment);
});

// Minimal API to create a new order
app.MapPost("/payments", async (IPaymentService paymentService, ProcessPaymentRequest request) =>
{
    var payment = await paymentService.ProcessPaymentAsync(request);

    return Results.Created($"/payments/{payment.Id}", payment);
});

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (context.Database.GetPendingMigrations().Any())
{
    await context.Database.MigrateAsync();
}

app.Run();
