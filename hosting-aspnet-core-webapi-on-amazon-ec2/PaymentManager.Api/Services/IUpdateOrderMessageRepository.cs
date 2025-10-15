namespace PaymentManager.Api.Services
{
    public interface IUpdateOrderMessageRepository
    {
        Task UpdateProcessedPaymentOrderMessage(Guid orderId);
        Task UpdatePaidPaymentOrderMessage(Guid orderId);
    }
}
