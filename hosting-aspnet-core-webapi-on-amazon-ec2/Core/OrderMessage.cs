namespace Core
{
    public class OrderMessage
    {
        public Guid OrderId { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    }
}
