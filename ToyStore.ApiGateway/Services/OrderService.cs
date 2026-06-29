using ToyStore.ApiGateway.Entities;
using ToyStore.ApiGateway.Repositories;
using ToyStore.Contracts.Enums;
using ToyStore.Contracts.Requests;
using ToyStore.Contracts.Responses;

namespace ToyStore.ApiGateway.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        // Calcula o total
        var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);

        // Cria a entidade Order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Cria os itens
        foreach (var itemRequest in request.Items)
        {
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ToyName = itemRequest.ToyName,
                Quantity = itemRequest.Quantity,
                UnitPrice = itemRequest.UnitPrice
            };

            order.Items.Add(orderItem);
        }

        // Persiste
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        // Mapeia para response
        return MapToResponse(order);
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        return order == null ? null : MapToResponse(order);
    }

    // Mapeamento manual
    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ToyName = i.ToyName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}