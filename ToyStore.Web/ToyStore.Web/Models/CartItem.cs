namespace ToyStore.Web.Models;

/// <summary>
/// Representa um item dentro do carrinho de compras.
/// Nesta etapa os dados são mockados; futuramente refletirá o estado real do carrinho.
/// </summary>
public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public decimal Total => UnitPrice * Quantity;
}
