namespace PaymentManager.Api.Domain
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public double Price { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}