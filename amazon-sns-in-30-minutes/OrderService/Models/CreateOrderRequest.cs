namespace OrderService.Models;

public record CreateOrderRequest(int OrderId, int CustomerId, List<ProductDetail> ProductDetails);
