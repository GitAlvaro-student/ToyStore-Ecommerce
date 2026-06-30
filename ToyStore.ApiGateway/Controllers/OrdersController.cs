using Microsoft.AspNetCore.Mvc;
using ToyStore.ApiGateway.Services;
using ToyStore.Contracts.Commands;
using ToyStore.Contracts.Requests;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;
using ToyStore.Infrastructure.Messaging.ServiceBus.Configurations;

namespace ToyStore.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMessagePublisher _publisher;

    public OrdersController(IOrderService orderService, IMessagePublisher publisher)
    {
        _orderService = orderService;
        _publisher = publisher;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var response = await _orderService.CreateOrderAsync(request);

        var command = new CreatePaymentCommand
        {
            OrderId = response.Id,
            CustomerName = response.CustomerName,
            TotalAmount = response.TotalAmount,
            CreatedAt = response.CreatedAt
        };

        await _publisher.PublishToQueueAsync(command, ServiceBusQueues.PaymentQueue);

        return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var response = await _orderService.GetOrderByIdAsync(id);

        if (response == null)
            return NotFound();

        return Ok(response);
    }
}