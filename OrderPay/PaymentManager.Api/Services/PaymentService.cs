using Core;
using Microsoft.EntityFrameworkCore;
using PaymentManager.Api.Domain;
using PaymentManager.Api.DTOs;

namespace PaymentManager.Api.Services
{
    public class PaymentService(AppDbContext dbContext, IUpdateOrderMessageRepository updateOrderMessageRepository) : IPaymentService
    {
        public async Task<List<Payment>> GetPaymentsAsync()
        {
            var payments = await dbContext.Payments.ToListAsync();
            return payments;
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

        public async Task<Payment> ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            var payment = await dbContext.Payments.FindAsync(request.Id);
            if (payment is null)
                return null;

            payment.Status = "Paid";
            payment.PaidAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            await updateOrderMessageRepository.UpdatePaidPaymentOrderMessage(payment.OrderId);

            return payment;
        }

        public async Task<Payment?> GetPaymentAsync(Guid id)
        {
            var payment = await dbContext.Payments.FindAsync(id);
            return payment ?? null;
        }
    }
}
