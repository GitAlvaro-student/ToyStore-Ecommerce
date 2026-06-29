namespace ToyStore.Contracts.Responses;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public string ToyName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}