namespace ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;

/// <summary>
/// Contrato genérico para publicação de mensagens.
/// Agnóstico ao tipo de mensagem — funciona com qualquer comando ou evento.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica uma mensagem serializada em JSON na fila informada.
    /// </summary>
    Task PublishToQueueAsync<T>(T message, string queueName) where T : class;

    /// <summary>
    /// Publica uma mensagem em um Topic.
    /// Todas as Subscriptions ativas receberão uma cópia independente.
    /// </summary>
    Task PublishToTopicAsync<T>(T message, string topicName) where T : class;
}