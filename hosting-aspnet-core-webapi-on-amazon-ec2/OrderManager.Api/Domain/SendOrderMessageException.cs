namespace OrderManager.Api.Domain
{
    public class SendOrderMessageException
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string ErrorDescription { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool RetrySuccess { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
