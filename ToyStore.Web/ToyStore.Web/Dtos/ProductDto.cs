namespace ToyStore.Web.Dtos
{
    /// <summary>
    /// DTO específico do frontend para representar um produto vindo da API.
    /// Espelha o contrato de ToyStore.ApiGateway.Products.ProductResponse,
    /// mas é mantido separado para o frontend não depender do assembly do backend.
    /// </summary>
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
