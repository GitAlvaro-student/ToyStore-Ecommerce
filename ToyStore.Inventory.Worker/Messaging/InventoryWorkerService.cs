using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyStore.Infrastructure.Messaging.ServiceBus.Configurations;

namespace ToyStore.Inventory.Worker.Messaging
{
    public class InventoryWorkerService : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InventoryWorkerService> _logger;
        private ServiceBusProcessor? _processor;

        public InventoryWorkerService(
            ServiceBusClient client,
            IServiceScopeFactory scopeFactory,
            ILogger<InventoryWorkerService> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Inventory Worker iniciado. Conectando à subscription: {Topic}/{Sub}",
                ServiceBusTopics.PaymentEvents.TopicName,
                ServiceBusTopics.PaymentEvents.Subscriptions.Inventory);

            // Para Subscriptions o processor recebe Topic + Subscription
            _processor = _client.CreateProcessor(
                ServiceBusTopics.PaymentEvents.TopicName,
                ServiceBusTopics.PaymentEvents.Subscriptions.Inventory,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

            _processor.ProcessMessageAsync += async args =>
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<InventoryMessageConsumer>();
                await consumer.ProcessMessageAsync(args);
            };

            _processor.ProcessErrorAsync += async args =>
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<InventoryMessageConsumer>();
                await consumer.ProcessErrorAsync(args);
            };

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("Inventory Worker conectado e aguardando eventos.");

            try { await Task.Delay(Timeout.Infinite, stoppingToken); }
            catch (TaskCanceledException) { }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inventory Worker encerrando...");

            if (_processor is not null)
            {
                await _processor.StopProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Inventory Worker encerrado.");
        }
    }
}
