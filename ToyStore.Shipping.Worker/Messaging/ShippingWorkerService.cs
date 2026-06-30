using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyStore.Infrastructure.Messaging.ServiceBus.Configurations;

namespace ToyStore.Shipping.Worker.Messaging
{
    public class ShippingWorkerService : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ShippingWorkerService> _logger;
        private ServiceBusProcessor? _processor;

        public ShippingWorkerService(
            ServiceBusClient client,
            IServiceScopeFactory scopeFactory,
            ILogger<ShippingWorkerService> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Shipping Worker iniciado. Conectando à subscription: {Topic}/{Sub}",
                ServiceBusTopics.PaymentEvents.TopicName,
                ServiceBusTopics.PaymentEvents.Subscriptions.Shipping);

            _processor = _client.CreateProcessor(
                ServiceBusTopics.PaymentEvents.TopicName,
                ServiceBusTopics.PaymentEvents.Subscriptions.Shipping,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

            _processor.ProcessMessageAsync += async args =>
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<ShippingMessageConsumer>();
                await consumer.ProcessMessageAsync(args);
            };

            _processor.ProcessErrorAsync += async args =>
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<ShippingMessageConsumer>();
                await consumer.ProcessErrorAsync(args);
            };

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("Shipping Worker conectado e aguardando eventos.");

            try { await Task.Delay(Timeout.Infinite, stoppingToken); }
            catch (TaskCanceledException) { }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shipping Worker encerrando...");

            if (_processor is not null)
            {
                await _processor.StopProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Shipping Worker encerrado.");
        }
    }
}
