namespace OrderService.Models;

public class OrderCreatedNotification
{
    public int OrderId { get; init; }
    public int CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<ProductDetail> ProductDetails { get; init; }

    public OrderCreatedNotification(int orderId, int customerId, List<ProductDetail> productDetails)
    {
        OrderId = orderId;
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        ProductDetails = productDetails;
    }
}