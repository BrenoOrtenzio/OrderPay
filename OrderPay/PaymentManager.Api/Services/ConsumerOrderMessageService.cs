using Confluent.Kafka;
using Core;
using System.Text.Json;

namespace PaymentManager.Api.Services
{
    public class ConsumerOrderMessageService : BackgroundService
    {
        private readonly ILogger<ConsumerOrderMessageService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerOrderMessageService(ILogger<ConsumerOrderMessageService> logger, IServiceScopeFactory scopeFactory)
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
                BootstrapServers = "98.92.168.150:9092",
                GroupId = "payment-api-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("Queue_Orders");

            _logger.LogInformation("Consumer Kafka iniciado...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(stoppingToken);

                        var json = cr.Message.Value;
                        var mensagem = JsonSerializer.Deserialize<OrderMessage>(json);

                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var updateRepo = scope.ServiceProvider.GetRequiredService<IUpdateOrderMessageRepository>();

                        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                        await paymentService.CreatePayment(mensagem);
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
