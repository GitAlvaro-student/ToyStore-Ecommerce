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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ServiceBusMessagePublisher(
        ServiceBusClient client,
        ILogger<ServiceBusMessagePublisher> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task PublishToQueueAsync<T>(T message, string queueName) where T : class
    {
        await SendAsync(message, queueName, "queue");
    }

    public async Task PublishToTopicAsync<T>(T message, string topicName) where T : class
    {
        await SendAsync(message, topicName, "topic");
    }

    // Lógica de envio reutilizada — Queue e Topic usam o mesmo ServiceBusSender
    private async Task SendAsync<T>(T message, string entityPath, string entityType) where T : class
    {
        _logger.LogInformation(
            "Publicando {MessageType} no {EntityType} '{EntityPath}'.",
            typeof(T).Name, entityType, entityPath);

        try
        {
            await using var sender = _client.CreateSender(entityPath);

            var json = JsonSerializer.Serialize(message, JsonOptions);

            var serviceBusMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                Subject = typeof(T).Name   // facilita filtros futuros por tipo
            };

            await sender.SendMessageAsync(serviceBusMessage);

            _logger.LogInformation(
                "{MessageType} (MessageId: {MessageId}) enviado com sucesso para {EntityType} '{EntityPath}'.",
                typeof(T).Name, serviceBusMessage.MessageId, entityType, entityPath);
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(ex,
                "Erro do Service Bus ao publicar {MessageType} em '{EntityPath}'. Reason: {Reason}",
                typeof(T).Name, entityPath, ex.Reason);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro inesperado ao publicar {MessageType} em '{EntityPath}'.",
                typeof(T).Name, entityPath);
            throw;
        }
    }
}