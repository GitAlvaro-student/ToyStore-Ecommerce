using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyStore.ApiGateway.Services;
using ToyStore.Contracts.Commands;
using ToyStore.Contracts.Enums;

namespace ToyStore.Payment.Worker.Messaging
{
    /// <summary>
    /// Responsável pela simulação do processamento de pagamento e atualização do pedido.
    /// Isolado do consumidor para facilitar a evolução nas próximas etapas.
    /// </summary>
    public class PaymentProcessor
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentProcessor> _logger;

        // Semente fixa garante comportamento determinístico nos testes
        private static readonly Random _random = new(42);

        public PaymentProcessor(
            IOrderService orderService,
            ILogger<PaymentProcessor> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task ProcessAsync(CreatePaymentCommand command)
        {
            _logger.LogInformation(
                "Iniciando processamento do pagamento | OrderId: {OrderId} | Total: {TotalAmount:C}",
                command.OrderId,
                command.TotalAmount);

            // Marca o pedido como em processamento
            await _orderService.UpdateOrderStatusAsync(command.OrderId, OrderStatus.PaymentProcessing);

            // Simula latência de um gateway de pagamento real
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            // Simula aprovação: 80% aprovado, 20% reprovado
            var approved = _random.Next(1, 101) <= 80;
            var resultStatus = approved ? OrderStatus.Paid : OrderStatus.PaymentFailed;

            _logger.LogInformation(
                "Resultado do pagamento | OrderId: {OrderId} | Resultado: {Result}",
                command.OrderId,
                approved ? "APROVADO" : "REPROVADO");

            // Atualiza o status final no banco
            await _orderService.UpdateOrderStatusAsync(command.OrderId, resultStatus);

            _logger.LogInformation(
                "Pedido {OrderId} atualizado para {Status}.",
                command.OrderId,
                resultStatus);
        }
    }
}
