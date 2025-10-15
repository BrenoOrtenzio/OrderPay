using Microsoft.EntityFrameworkCore;
using PaymentManager.Api.Domain;

namespace PaymentManager.Api;

public class AppDbContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }
    public DbSet<UpdateOrderMessageException> UpdateOrderMessageExceptions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}