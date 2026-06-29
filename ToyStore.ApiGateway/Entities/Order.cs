using ToyStore.Contracts.Enums;

namespace ToyStore.ApiGateway.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relacionamento
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
