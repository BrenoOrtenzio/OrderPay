using OrderManager.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace OrderManager.Api;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<SendOrderMessageException> SendOrderMessageExceptions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}