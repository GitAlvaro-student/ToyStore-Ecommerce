using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToyStore.ApiGateway.Services;
using ToyStore.Contracts.Enums;
using ToyStore.Contracts.Events;

namespace ToyStore.Shipping.Worker.Messaging
{
    public class ShippingMessageConsumer
    {
        private readonly ILogger<ShippingMessageConsumer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ShippingMessageConsumer(ILogger<ShippingMessageConsumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            _logger.LogInformation(
                "Shipping Worker | Mensagem recebida | MessageId: {MessageId}",
                args.Message.MessageId);

            try
            {
                var json = args.Message.Body.ToString();

                var paymentEvent = JsonSerializer.Deserialize<PaymentApprovedEvent>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (paymentEvent is null)
                    throw new InvalidOperationException("Falha na desserialização: evento nulo.");

                _logger.LogInformation(
                    "PaymentApprovedEvent recebido | OrderId: {OrderId} | Cliente: {CustomerName} | Total: {TotalAmount:C} | AprovadoEm: {ApprovedAt}",
                    paymentEvent.OrderId,
                    paymentEvent.CustomerName,
                    paymentEvent.TotalAmount,
                    paymentEvent.ApprovedAt);

                // Simula preparação para envio
                _logger.LogInformation(
                    "Simulando preparação de envio para OrderId: {OrderId}...",
                    paymentEvent.OrderId);

                await Task.Delay(300);

                _logger.LogInformation(
                    "Envio preparado com sucesso para OrderId: {OrderId}.",
                    paymentEvent.OrderId);

                using var scope = _serviceScopeFactory.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                Thread.Sleep(1000);
                await orderService.UpdateOrderStatusAsync(paymentEvent.OrderId, OrderStatus.Shipped);

                await args.CompleteMessageAsync(args.Message);

                _logger.LogInformation(
                    "Mensagem {MessageId} concluída com CompleteMessageAsync.",
                    args.Message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao processar mensagem {MessageId}. Executando AbandonMessageAsync.",
                    args.Message.MessageId);

                await args.AbandonMessageAsync(args.Message);
            }
        }

        public Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception,
                "Erro no Shipping Processor | Fonte: {ErrorSource} | Entidade: {EntityPath}",
                args.ErrorSource, args.EntityPath);

            return Task.CompletedTask;
        }
    }
}
