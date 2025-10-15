using OrderManager.Api.Domain;
using OrderManager.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OrderManager.Api.Services;

public class OrderService(AppDbContext dbContext) : IOrderService
{
    public async Task<Guid> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order()
        {
            Description = request.Description,
            Price = request.Price,
        };

        await dbContext.AddAsync(order);
        await dbContext.SaveChangesAsync();
        return order.Id;
    }

    public async Task<Order?> GetOrderAsync(Guid id)
    {
        var order = await dbContext.Orders.FindAsync(id);
        return order ?? null;
    }

    public async Task<List<Order>> GetOrderAsync()
    {
        var orders = await dbContext.Orders.ToListAsync();
        return orders;
    }
}
