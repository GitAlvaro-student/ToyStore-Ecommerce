using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToyStore.Infrastructure.Messaging.ServiceBus.Configurations;

namespace ToyStore.Payment.Worker.Messaging
{
    /// <summary>
    /// Hosted Service responsável pelo ciclo de vida do ServiceBusProcessor.
    /// Inicia o consumo no StartAsync e para no StopAsync.
    /// </summary>
    public class PaymentWorkerService : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PaymentWorkerService> _logger;
        private ServiceBusProcessor? _processor;

        public PaymentWorkerService(
            ServiceBusClient client,
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentWorkerService> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payment Worker iniciado. Conectando à fila: {Queue}",
                ServiceBusQueues.PaymentQueue);

            // Cria o processor para a fila
            _processor = _client.CreateProcessor(ServiceBusQueues.PaymentQueue, new ServiceBusProcessorOptions
            {
                // Controle manual do Complete/Abandon
                AutoCompleteMessages = false,

                // Quantas mensagens processar em paralelo
                MaxConcurrentCalls = 1
            });

            // Registra os handlers
            _processor.ProcessMessageAsync += async args =>
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<PaymentMessageConsumer>();
                await consumer.ProcessMessageAsync(args);
            };

            _processor.ProcessErrorAsync += async args =>
            {
                using var scope = _scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<PaymentMessageConsumer>();
                await consumer.ProcessErrorAsync(args);
            };

            // Inicia o processamento
            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("Payment Worker conectado e aguardando mensagens na fila '{Queue}'.",
                ServiceBusQueues.PaymentQueue);

            // Mantém o Worker vivo até receber sinal de cancelamento
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Comportamento esperado ao encerrar
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payment Worker encerrando...");

            if (_processor is not null)
            {
                await _processor.StopProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);

            _logger.LogInformation("Payment Worker encerrado.");
        }
    }
}
