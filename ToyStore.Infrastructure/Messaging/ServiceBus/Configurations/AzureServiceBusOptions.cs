namespace ToyStore.Infrastructure.Messaging.AzureServiceBus.Configurations;

/// <summary>
/// Opções de configuração do Azure Service Bus.
/// Será expandida nas próximas etapas (ex: TopicName, RetryOptions, etc).
/// </summary>
public class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBus";

    public string ConnectionString { get; set; } = string.Empty;
}