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
    /// Responsável pelo recebimento e orquestração do processamento das mensagens da payment-queue.
    /// Delega o processamento de negócio ao PaymentProcessor.
    /// </summary>
    public class PaymentMessageConsumer
    {
        private readonly PaymentProcessor _processor;
        private readonly ILogger<PaymentMessageConsumer> _logger;

        public PaymentMessageConsumer(
            PaymentProcessor processor,
            ILogger<PaymentMessageConsumer> logger)
        {
            _processor = processor;
            _logger = logger;
        }

        public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            _logger.LogInformation(
                "Mensagem recebida | MessageId: {MessageId} | Horário: {ReceivedAt}",
                args.Message.MessageId,
                DateTimeOffset.UtcNow);

            try
            {
                var json = args.Message.Body.ToString();

                var command = JsonSerializer.Deserialize<CreatePaymentCommand>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (command is null)
                    throw new InvalidOperationException("Falha na desserialização: comando nulo.");

                _logger.LogInformation(
                    "CreatePaymentCommand desserializado | OrderId: {OrderId} | Cliente: {CustomerName}",
                    command.OrderId,
                    command.CustomerName);

                // Delega o processamento
                await _processor.ProcessAsync(command);

                // CompleteMessageAsync SOMENTE após tudo concluído com sucesso
                await args.CompleteMessageAsync(args.Message);

                _logger.LogInformation(
                    "Mensagem {MessageId} concluída com CompleteMessageAsync.",
                    args.Message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar mensagem {MessageId}. Executando AbandonMessageAsync.",
                    args.Message.MessageId);

                await args.AbandonMessageAsync(args.Message);
            }
        }

        public Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Erro no ServiceBusProcessor | Fonte: {ErrorSource} | Entidade: {EntityPath}",
                args.ErrorSource,
                args.EntityPath);

            return Task.CompletedTask;
        }
    }
}
