namespace ToyStore.Contracts.Requests;

public class CreateOrderItemRequest
{
    public string ToyName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}