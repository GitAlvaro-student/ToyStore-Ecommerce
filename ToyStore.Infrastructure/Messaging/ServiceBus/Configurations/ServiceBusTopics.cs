namespace ToyStore.Infrastructure.Messaging.ServiceBus.Configurations
{
    /// <summary>
    /// Centraliza os nomes dos Topics e Subscriptions do Azure Service Bus.
    /// Evita strings literais espalhadas pelo código.
    /// Adicionar novos Topics e Subscriptions aqui conforme o sistema evoluir.
    /// </summary>
    public static class ServiceBusTopics
    {
        public static class PaymentEvents
        {
            public const string TopicName = "payment-events-topic";

            public static class Subscriptions
            {
                public const string Inventory = "inventory-sub";
                public const string Shipping = "shipping-sub";
            }
        }
    }
}
