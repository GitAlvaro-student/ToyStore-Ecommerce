using Microsoft.EntityFrameworkCore;
using ToyStore.ApiGateway.Entities;
using ToyStore.ApiGateway.Persistence;

namespace ToyStore.ApiGateway.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ToyStoreDbContext _context;

    public OrderRepository(ToyStoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}