using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyStore.ApiGateway.Services;
using ToyStore.Contracts.Commands;
using ToyStore.Contracts.Enums;
using ToyStore.Contracts.Events;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;
using ToyStore.Infrastructure.Messaging.ServiceBus.Configurations;

namespace ToyStore.Payment.Worker.Messaging
{
    /// <summary>
    /// Responsável pela simulação do processamento de pagamento e atualização do pedido.
    /// Isolado do consumidor para facilitar a evolução nas próximas etapas.
    /// </summary>
    public class PaymentProcessor
    {
        private readonly IOrderService _orderService;
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<PaymentProcessor> _logger;

        private static readonly Random _random = new(42);

        public PaymentProcessor(
            IOrderService orderService,
            IMessagePublisher publisher,
            ILogger<PaymentProcessor> logger)
        {
            _orderService = orderService;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task ProcessAsync(CreatePaymentCommand command)
        {
            _logger.LogInformation(
                "Iniciando processamento | OrderId: {OrderId} | Total: {TotalAmount:C}",
                command.OrderId, command.TotalAmount);

            await _orderService.UpdateOrderStatusAsync(command.OrderId, OrderStatus.PaymentProcessing);

            await Task.Delay(500); // simula latência do gateway

            var approved = _random.Next(1, 101) <= 80;

            _logger.LogInformation(
                "Resultado do pagamento | OrderId: {OrderId} | Resultado: {Result}",
                command.OrderId, approved ? "APROVADO" : "REPROVADO");

            if (approved)
            {
                await _orderService.UpdateOrderStatusAsync(command.OrderId, OrderStatus.Paid);

                // Publica o evento no Topic ANTES do CompleteMessageAsync
                // (o Complete ocorre no Consumer após este método retornar com sucesso)
                var approvedEvent = new PaymentApprovedEvent
                {
                    OrderId = command.OrderId,
                    CustomerName = command.CustomerName,
                    TotalAmount = command.TotalAmount,
                    ApprovedAt = DateTime.UtcNow
                };

                await _publisher.PublishToTopicAsync(
                    approvedEvent,
                    ServiceBusTopics.PaymentEvents.TopicName);

                _logger.LogInformation(
                    "PaymentApprovedEvent publicado no Topic | OrderId: {OrderId}",
                    command.OrderId);
            }
            else
            {
                await _orderService.UpdateOrderStatusAsync(command.OrderId, OrderStatus.PaymentFailed);
            }
        }
    }
}
