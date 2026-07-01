using Microsoft.EntityFrameworkCore;
using ToyStore.ApiGateway.Entities;

namespace ToyStore.ApiGateway.Persistence;

public class ToyStoreDbContext : DbContext
{
    public ToyStoreDbContext(DbContextOptions<ToyStoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToyStoreDbContext).Assembly);
    }
}