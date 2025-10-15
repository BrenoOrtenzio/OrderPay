namespace OrderManager.Api.Domain;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Description { get; set; }
    public double Price { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Active, Completed, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}