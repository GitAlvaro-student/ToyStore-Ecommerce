using ToyStore.Contracts.Enums;
using ToyStore.Contracts.Requests;
using ToyStore.Contracts.Responses;

namespace ToyStore.ApiGateway.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse?> GetOrderByIdAsync(Guid id);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
}