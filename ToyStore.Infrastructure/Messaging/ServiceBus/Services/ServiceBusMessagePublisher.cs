using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;

namespace ToyStore.Infrastructure.Messaging.AzureServiceBus.Services;

/// <summary>
/// Implementação de IMessagePublisher utilizando Azure Service Bus SDK oficial.
/// Serializa objetos para JSON e os envia para a fila informada.
/// </summary>
public class ServiceBusMessagePublisher : IMessagePublisher
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<ServiceBusMessagePublisher> _logger;

    public ServiceBusMessagePublisher(
        ServiceBusClient client,
        ILogger<ServiceBusMessagePublisher> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string queueName) where T : class
    {
        _logger.LogInformation(
            "Publicando mensagem do tipo {MessageType} na fila {QueueName}.",
            typeof(T).Name,
            queueName);

        try
        {
            // Cria um sender para a fila específica
            // O SDK reutiliza conexões internamente — não há overhead em criar o sender aqui
            await using var sender = _client.CreateSender(queueName);

            // Serializa o objeto para JSON
            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Cria a mensagem do Service Bus
            var serviceBusMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            };

            // Envia para a fila
            await sender.SendMessageAsync(serviceBusMessage);

            _logger.LogInformation(
                "Mensagem {MessageId} do tipo {MessageType} enviada com sucesso para a fila {QueueName}.",
                serviceBusMessage.MessageId,
                typeof(T).Name,
                queueName);
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(
                ex,
                "Erro do Service Bus ao publicar mensagem do tipo {MessageType} na fila {QueueName}. Reason: {Reason}",
                typeof(T).Name,
                queueName,
                ex.Reason);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado ao publicar mensagem do tipo {MessageType} na fila {QueueName}.",
                typeof(T).Name,
                queueName);

            throw;
        }
    }
}