namespace ToyStore.Contracts.Events
{
    /// <summary>
    /// Evento publicado pelo Payment Worker após aprovação do pagamento.
    /// Consumido de forma independente pelo Inventory Worker e Shipping Worker
    /// através de suas respectivas Subscriptions no payment-events-topic.
    /// </summary>
    public class PaymentApprovedEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime ApprovedAt { get; set; }
    }
}
