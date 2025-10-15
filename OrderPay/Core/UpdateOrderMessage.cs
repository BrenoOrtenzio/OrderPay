namespace Core
{
    public class UpdateOrderMessage
    {
        public Guid OrderId { get; set; }
        public UpdateType UpdateType { get; set; }
    }

    public enum UpdateType
    {
        PaymentProcessed = 1,
        PaymentPaid = 2
    }
}
