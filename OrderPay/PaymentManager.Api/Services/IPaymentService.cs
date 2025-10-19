using Core;
using PaymentManager.Api.Domain;
using PaymentManager.Api.DTOs;

namespace PaymentManager.Api.Services
{
    public interface IPaymentService
    {
        Task<List<Payment>> GetPaymentsAsync();
        Task<Payment?> GetPaymentAsync(Guid id);
        Task<Payment> ProcessPaymentAsync(ProcessPaymentRequest request);
        Task<Guid> CreatePayment(OrderMessage message);
    }
}
