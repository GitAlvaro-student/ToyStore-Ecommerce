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
    /// <typeparam name="T">Tipo da mensagem a ser publicada.</typeparam>
    /// <param name="message">Objeto a ser serializado e enviado.</param>
    /// <param name="queueName">Nome da fila de destino.</param>
    Task PublishAsync<T>(T message, string queueName) where T : class;
}