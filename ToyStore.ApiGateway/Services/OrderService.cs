using ToyStore.ApiGateway.Entities;
using ToyStore.ApiGateway.Repositories;
using ToyStore.Contracts.Commands;
using ToyStore.Contracts.Enums;
using ToyStore.Contracts.Requests;
using ToyStore.Contracts.Responses;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;
using ToyStore.Infrastructure.Messaging.ServiceBus.Configurations;

namespace ToyStore.ApiGateway.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IMessagePublisher publisher,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _publisher = publisher;
        _logger = logger;
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

        _logger.LogInformation("Pedido {OrderId} criado com sucesso.", order.Id);

        var command = new CreatePaymentCommand
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt
        };

        await _publisher.PublishAsync(command, ServiceBusQueues.PaymentQueue);

        _logger.LogInformation(
            "CreatePaymentCommand publicado para o pedido {OrderId}.", order.Id);

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

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
        {
            _logger.LogWarning(
                "Tentativa de atualizar status do pedido {OrderId} que não foi encontrado.",
                orderId);
            return;
        }

        var previousStatus = order.Status;
        order.Status = status;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Pedido {OrderId} atualizado: {PreviousStatus} → {NewStatus}.",
            orderId,
            previousStatus,
            status);
    }
}