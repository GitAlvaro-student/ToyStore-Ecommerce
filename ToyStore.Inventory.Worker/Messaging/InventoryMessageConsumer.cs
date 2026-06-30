using Azure.Messaging.ServiceBus;
using System.Configuration;
using System.Text.Json;
using ToyStore.Contracts.Events;

namespace ToyStore.Inventory.Worker.Messaging;

public class InventoryMessageConsumer
{
    private readonly ILogger<InventoryMessageConsumer> _logger;
    private readonly bool _simulateFailure;

    public InventoryMessageConsumer(ILogger<InventoryMessageConsumer> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _simulateFailure = configuration.GetValue<bool>("InventorySimulation:SimulateFailure");
    }

    public async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        // DeliveryCount indica quantas vezes esta mensagem já foi entregue
        // (1 = primeira tentativa, 2 = segunda tentativa, e assim por diante)
        var deliveryCount = args.Message.DeliveryCount;

        _logger.LogInformation(
            "Inventory Worker | Mensagem recebida | MessageId: {MessageId} | DeliveryCount: {DeliveryCount} (Tentativa {Attempt})",
            args.Message.MessageId,
            deliveryCount,
            deliveryCount);

        try
        {
            var json = args.Message.Body.ToString();

            var paymentEvent = JsonSerializer.Deserialize<PaymentApprovedEvent>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (paymentEvent is null)
                throw new InvalidOperationException("Falha na desserialização: evento nulo.");

            _logger.LogInformation(
                "PaymentApprovedEvent recebido | OrderId: {OrderId} | Cliente: {CustomerName} | DeliveryCount: {DeliveryCount}",
                paymentEvent.OrderId,
                paymentEvent.CustomerName,
                deliveryCount);

            // ─── SIMULAÇÃO DE FALHA ────────────────────────────────────────────
            // Controlada por appsettings.json → InventorySimulation:SimulateFailure
            if (_simulateFailure)
            {
                throw new InvalidOperationException(
                    $"[SIMULAÇÃO] Falha proposital ao reservar estoque para OrderId {paymentEvent.OrderId} " +
                    $"(Tentativa {deliveryCount}).");
            }
            // ────────────────────────────────────────────────────────────────────

            _logger.LogInformation(
                "Simulando reserva de estoque para OrderId: {OrderId}...",
                paymentEvent.OrderId);

            await Task.Delay(300);

            _logger.LogInformation(
                "Estoque reservado com sucesso para OrderId: {OrderId}.",
                paymentEvent.OrderId);

            await args.CompleteMessageAsync(args.Message);

            _logger.LogInformation(
                "Mensagem {MessageId} concluída com CompleteMessageAsync.",
                args.Message.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao processar mensagem {MessageId} (DeliveryCount: {DeliveryCount}). " +
                "Executando AbandonMessageAsync — o Azure Service Bus tentará reentregar a mensagem.",
                args.Message.MessageId,
                deliveryCount);

            // NÃO usar DeadLetterMessageAsync aqui — queremos observar
            // o comportamento automático do Azure ao atingir MaxDeliveryCount
            await args.AbandonMessageAsync(args.Message);
        }
    }

    public Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception,
            "Erro no Inventory Processor | Fonte: {ErrorSource} | Entidade: {EntityPath}",
            args.ErrorSource, args.EntityPath);

        return Task.CompletedTask;
    }
}