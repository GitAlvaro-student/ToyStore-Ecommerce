using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Configurations;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Interfaces;
using ToyStore.Infrastructure.Messaging.AzureServiceBus.Services;

namespace ToyStore.Infrastructure.Messaging.AzureServiceBus.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra toda a infraestrutura do Azure Service Bus no container de DI.
    /// </summary>
    public static IServiceCollection AddAzureServiceBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Lê e valida as opções
        var options = configuration
            .GetSection(AzureServiceBusOptions.SectionName)
            .Get<AzureServiceBusOptions>();

        if (options is null || string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new InvalidOperationException(
                "A configuração 'AzureServiceBus:ConnectionString' não foi encontrada ou está vazia.");

        // Registra as opções via Options Pattern para uso futuro
        services.Configure<AzureServiceBusOptions>(
            configuration.GetSection(AzureServiceBusOptions.SectionName));

        // Registra o ServiceBusClient como Singleton
        // O SDK recomenda uma única instância por aplicação — ele gerencia o pool de conexões internamente
        services.AddSingleton(_ => new ServiceBusClient(options.ConnectionString));

        // Registra o publisher como Singleton (depende do ServiceBusClient que também é Singleton)
        services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();

        return services;
    }

    public static IServiceCollection AddMessaging(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddAzureServiceBus(configuration);
        return services;
    }
}