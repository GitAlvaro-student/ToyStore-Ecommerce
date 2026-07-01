namespace ToyStore.Web.Dtos
{

    public class CreateOrderItemDto
    {
        public string ToyName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
