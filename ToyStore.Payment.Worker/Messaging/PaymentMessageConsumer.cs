using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToyStore.Contracts.Commands;

namespace ToyStore.Payment.Worker.Messaging
{
    /// <summary>
    /// Responsável pelo processamento de cada mensagem recebida da payment-queue.
    /// Separado do WorkerService para facilitar testes e evolução futura.
    /// </summary>
    public class PaymentMessageConsumer
    {
        private readonly ILogger<PaymentMessageConsumer> _logger;
        private readonly PaymentProcessor _processor;

        public PaymentMessageConsumer(ILogger<PaymentMessageConsumer> logger, PaymentProcessor processor)
        {
            _logger = logger;
            _processor = processor;
        }

        public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            _logger.LogInformation(
                "Mensagem recebida da fila. MessageId: {MessageId} | Horário: {ReceivedAt}",
                args.Message.MessageId,
                DateTimeOffset.UtcNow);

            try
            {
                // Desserializa o JSON de volta para o comando
                var json = args.Message.Body.ToString();

                var command = JsonSerializer.Deserialize<CreatePaymentCommand>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (command is null)
                    throw new InvalidOperationException("Falha na desserialização: comando nulo.");

                // Registra as informações recebidas
                _logger.LogInformation(
                    "CreatePaymentCommand processado | OrderId: {OrderId} | Cliente: {CustomerName} | Total: {TotalAmount:C} | CriadoEm: {CreatedAt}",
                    command.OrderId,
                    command.CustomerName,
                    command.TotalAmount,
                    command.CreatedAt);

                await _processor.ProcessAsync(command);

                // Conclui a mensagem — remove da fila permanentemente
                await args.CompleteMessageAsync(args.Message);

                _logger.LogInformation(
                    "Mensagem {MessageId} concluída com sucesso (CompleteMessageAsync).",
                    args.Message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar mensagem {MessageId}. Executando AbandonMessageAsync.",
                    args.Message.MessageId);

                // Abandona — a mensagem volta para a fila e o DeliveryCount é incrementado
                await args.AbandonMessageAsync(args.Message);
            }
        }

        public Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Erro no ServiceBusProcessor | Fonte: {ErrorSource} | Namespace: {Namespace} | Entidade: {EntityPath}",
                args.ErrorSource,
                args.FullyQualifiedNamespace,
                args.EntityPath);

            return Task.CompletedTask;
        }
    }
}
