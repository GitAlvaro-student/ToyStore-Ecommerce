namespace ToyStore.Web.Dtos;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatusDto Status { get; set; }
    public DateTime CreatedAt { get; set; }
}