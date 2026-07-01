namespace ToyStore.Web.Models;

/// <summary>
/// Representa um brinquedo exibido no frontend.
/// Modelo simplificado, apenas para construção da interface.
/// Será revisado/ajustado quando a integração com a API for implementada.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
