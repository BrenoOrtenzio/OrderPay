using OrderManager.Api.Domain;
using OrderManager.Api.DTOs;

namespace OrderManager.Api.Services;

public interface IOrderService
{
    Task<List<Order>> GetOrderAsync();
    Task<Order?> GetOrderAsync(Guid id);
    Task<Guid> CreateOrderAsync(CreateOrderRequest request);
}
