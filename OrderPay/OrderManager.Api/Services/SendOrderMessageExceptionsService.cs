using Core;
using System.Text.Json;

namespace OrderManager.Api.Services
{
    public class SendOrderMessageExceptionsService(AppDbContext dbContext, IOrderMessageRepository orderMessageRepository) : ISendOrderMessageExceptionsService
    {
        public async Task ReprocessOrders()
        {
            var ordersWithExceptions = dbContext.SendOrderMessageExceptions.Where(x => !x.RetrySuccess);

            foreach(var item in ordersWithExceptions)
            {
                var message = JsonSerializer.Deserialize<OrderMessage>(item.Message);

                await orderMessageRepository.SendOrderMessage(new Core.OrderMessage
                {
                    OrderId = item.OrderId,
                    Description = message.Description,
                    Price = message.Price,
                    CreatedAt = message.CreatedAt
                });

                item.RetrySuccess = true;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
