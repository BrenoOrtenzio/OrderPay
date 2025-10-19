using Confluent.Kafka;
using Core;
using System.Text.Json;

namespace OrderManager.Api.Services
{
    public class ConsumerUpdateOrderMessageService : BackgroundService
    {
        private readonly ILogger<ConsumerUpdateOrderMessageService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerUpdateOrderMessageService(ILogger<ConsumerUpdateOrderMessageService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        private async Task StartConsumerLoop(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "3.92.6.208:9092",
                GroupId = "order-api-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("UpdateOrderStatus_Queue");

            _logger.LogInformation("Consumer Kafka iniciado...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(stoppingToken);

                        var json = cr.Message.Value;
                        var mensagem = JsonSerializer.Deserialize<UpdateOrderMessage>(json);
                        if (mensagem is null)
                            continue;

                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var order = await dbContext.Orders.FindAsync(mensagem.OrderId);

                        if (order != null)
                        {
                            order.Status = mensagem.UpdateType == UpdateType.PaymentProcessed ? "Active" : "Completed";
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError("Erro ao consumir mensagem: {Reason}", ex.Error.Reason);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Encerrando consumer Kafka...");
                consumer.Close();
            }
        }
    }
}
