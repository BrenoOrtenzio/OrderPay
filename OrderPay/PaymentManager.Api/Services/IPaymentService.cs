using Core;
using PaymentManager.Api.Domain;
using PaymentManager.Api.DTOs;

namespace PaymentManager.Api.Services
{
    public interface IPaymentService
    {
        Task<Payment?> GetOrderAsync(Guid id);
        Task<Guid> ProcessPaymentAsync(ProcessPaymentRequest request);
        Task<Guid> CreatePayment(OrderMessage message);
    }
}
