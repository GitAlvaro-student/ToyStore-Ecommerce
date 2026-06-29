using Microsoft.AspNetCore.Mvc;
using ToyStore.ApiGateway.Services;
using ToyStore.Contracts.Requests;

namespace ToyStore.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var response = await _orderService.CreateOrderAsync(request);
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