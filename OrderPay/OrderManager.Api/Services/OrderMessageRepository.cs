using Confluent.Kafka;
using Core;
using OrderManager.Api.Domain;

namespace OrderManager.Api.Services
{
    public class OrderMessageRepository : IOrderMessageRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderMessageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendOrderMessage(OrderMessage mensagem)
        {
            var json = System.Text.Json.JsonSerializer.Serialize<OrderMessage>(mensagem);

            try
            {
                var config = new Confluent.Kafka.ProducerConfig
                {
                    BootstrapServers = "3.223.192.14:9092",
                    MessageTimeoutMs = 3000,             // tempo máximo para entrega
                    SocketTimeoutMs = 3000,              // timeout de conexão
                    Acks = Acks.Leader                   // confirmação mínima
                };


                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    var result = await producer.ProduceAsync("Queue_Orders", new Message<string, string> { Key = mensagem.OrderId.ToString(), Value = json });
                }
            }
            catch (Exception ex)
            {
                var error = $"Falha ao postar mensagem de gerar pagamento. Descrição: {ex.Message}";
                var orderException = new SendOrderMessageException()
                {
                    OrderId = mensagem.OrderId,
                    ErrorDescription = error,
                    Queue = "Queue_Orders",
                    Message = json,
                };

                _dbContext.SendOrderMessageExceptions.Add(orderException);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
