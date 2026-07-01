using Microsoft.AspNetCore.Mvc;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;

namespace ToyStore.ApiGateway.Controllers;

/// <summary>
/// Endpoint temporário para validar a infraestrutura do Azure Service Bus.
/// Remover após a conclusão dos testes.
/// </summary>
[ApiController]
[Route("api/servicebus")]
public class ServiceBusTestController : ControllerBase
{
    private readonly IMessagePublisher _publisher;

    public ServiceBusTestController(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    /// <summary>
    /// Envia uma mensagem de teste para a fila informada.
    /// </summary>
    [HttpPost("test")]
    public async Task<IActionResult> Test([FromBody] ServiceBusTestRequest request)
    {
        var testMessage = new
        {
            Source = "ToyStore.ApiGateway",
            SentAt = DateTime.UtcNow,
            Payload = request.Payload
        };

        await _publisher.PublishToQueueAsync(testMessage, request.QueueName);

        return Ok(new
        {
            Message = "Mensagem enviada com sucesso.",
            QueueName = request.QueueName,
            SentAt = DateTime.UtcNow
        });
    }
}

public record ServiceBusTestRequest(string QueueName, string Payload);