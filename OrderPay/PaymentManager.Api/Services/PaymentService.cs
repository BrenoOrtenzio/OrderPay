using Confluent.Kafka;
using Core;
using PaymentManager.Api.Domain;
using PaymentManager.Api.DTOs;

namespace PaymentManager.Api.Services
{
    public class PaymentService(AppDbContext dbContext, IUpdateOrderMessageRepository updateOrderMessageRepository) : IPaymentService
    {
        public async Task<Payment?> GetOrderAsync(Guid id)
        {
            var payment = await dbContext.Payments.FindAsync(id);
            return payment ?? null;
        }

        public async Task<Guid> CreatePayment(OrderMessage message)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = message.OrderId,
                Price = message.Price,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Payments.Add(payment);
            await dbContext.SaveChangesAsync();

            await updateOrderMessageRepository.UpdateProcessedPaymentOrderMessage(message.OrderId);

            return payment.Id;
        }

        public async Task<Guid> ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            var payment = await dbContext.Payments.FindAsync(request.Id);
            if (payment is null)
                return Guid.Empty;

            payment.Status = "Paid";
            payment.PaidAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            await updateOrderMessageRepository.UpdatePaidPaymentOrderMessage(payment.OrderId);

            return payment.Id;
        }
    }
}
