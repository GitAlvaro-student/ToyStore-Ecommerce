using ToyStore.Web.Dtos;

namespace ToyStore.Web.Services;

public class OrderServiceException : Exception
{
    public OrderServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Toda comunicação HTTP relacionada a pedidos passa por aqui.
/// O frontend apenas solicita a criação — o restante do processamento
/// ocorre de forma assíncrona no backend via Azure Service Bus.
/// </summary>
public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", dto);
            response.EnsureSuccessStatusCode();

            var order = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            return order!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao criar pedido para o cliente {CustomerName}.", dto.CustomerName);
            throw new OrderServiceException(
                "Não foi possível criar o pedido no momento. Tente novamente em instantes.",
                ex);
        }
    }

    public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/orders/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar pedido {OrderId}.", id);
            throw new OrderServiceException(
                "Não foi possível consultar o pedido no momento. Tente novamente em instantes.", ex);
        }
    }
}