namespace ToyStore.Web.Dtos;

public enum OrderStatusDto
{
    Pending = 0,
    PaymentProcessing = 1,
    Paid = 2,
    PaymentFailed = 3,
    Cancelled = 4,
    InventoryReserved = 5,
    Shipped = 6,
    Completed = 7
}