namespace ToyStore.ApiGateway.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ToyName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navegação
    public Order Order { get; set; } = null!;
}