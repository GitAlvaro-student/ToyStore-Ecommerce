using ToyStore.ApiGateway.Entities;

namespace ToyStore.ApiGateway.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task<int> SaveChangesAsync();
}