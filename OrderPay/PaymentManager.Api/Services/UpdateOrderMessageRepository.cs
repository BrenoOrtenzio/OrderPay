using Confluent.Kafka;
using Core;
using PaymentManager.Api.Domain;

namespace PaymentManager.Api.Services
{
    public class UpdateOrderMessageRepository : IUpdateOrderMessageRepository
    {
        private readonly AppDbContext _dbContext;

        public UpdateOrderMessageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateProcessedPaymentOrderMessage(Guid orderId)
        {
            var json = System.Text.Json.JsonSerializer.Serialize<UpdateOrderMessage>(new UpdateOrderMessage { OrderId = orderId, UpdateType = UpdateType.PaymentProcessed });

            try
            {
                var config = new Confluent.Kafka.ProducerConfig
                {
                    BootstrapServers = "3.92.6.208:9092",
                    MessageTimeoutMs = 3000,             // tempo máximo para entrega
                    SocketTimeoutMs = 3000,              // timeout de conexão
                    Acks = Acks.Leader                   // confirmação mínima
                };

                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    var result = await producer.ProduceAsync("UpdateOrderStatus_Queue", new Message<string, string> { Key = orderId.ToString(), Value = json });
                }
            }
            catch (Exception ex)
            {
                var error = $"Falha ao postar mensagem de atualização do pedido. UpdateType: {UpdateType.PaymentProcessed} - Descrição: {ex.Message}";
                var orderException = new UpdateOrderMessageException()
                {
                    OrderId = orderId,
                    ErrorDescription = error,
                    Queue = "UpdateOrderStatus_Queue",
                    Message = json,
                };

                _dbContext.UpdateOrderMessageExceptions.Add(orderException);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        public async Task UpdatePaidPaymentOrderMessage(Guid orderId)
        {
            var json = System.Text.Json.JsonSerializer.Serialize<UpdateOrderMessage>(new UpdateOrderMessage { OrderId = orderId, UpdateType = UpdateType.PaymentPaid });

            try
            {
                var config = new Confluent.Kafka.ProducerConfig
                {
                    BootstrapServers = "3.92.6.208:9092",
                    MessageTimeoutMs = 3000,             // tempo máximo para entrega
                    SocketTimeoutMs = 3000,              // timeout de conexão
                    Acks = Acks.Leader                   // confirmação mínima
                };

                using (var producer = new ProducerBuilder<string, string>(config).Build())
                {
                    var result = await producer.ProduceAsync("UpdateOrderStatus_Queue", new Message<string, string> { Key = orderId.ToString(), Value = json });
                }
            }
            catch (Exception ex)
            {
                var error = $"Falha ao postar mensagem de atualização do pedido. UpdateType: {UpdateType.PaymentPaid} - Descrição: {ex.Message}";
                var orderException = new UpdateOrderMessageException()
                {
                    OrderId = orderId,
                    ErrorDescription = error,
                    Queue = "UpdateOrderStatus_Queue",
                    Message = json,
                };

                _dbContext.UpdateOrderMessageExceptions.Add(orderException);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
