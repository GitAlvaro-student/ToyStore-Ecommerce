namespace ToyStore.ApiGateway.Entities
{
    /// <summary>
    /// Representa um brinquedo do catálogo. Não é uma entidade EF Core —
    /// existe apenas em memória, sem persistência em banco.
    /// </summary>
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
