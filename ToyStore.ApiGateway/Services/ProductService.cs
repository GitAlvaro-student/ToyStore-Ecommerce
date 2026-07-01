using ToyStore.ApiGateway.Entities;
using ToyStore.ApiGateway.Persistence;
using ToyStore.Contracts.Responses;

namespace ToyStore.ApiGateway.Services
{
    /// <summary>
    /// Serviço responsável por expor o catálogo estático de produtos.
    /// Não há acesso a banco de dados — os dados vêm de ProductCatalog.
    /// Os métodos são async apenas para manter a mesma assinatura usada
    /// pelos demais services da aplicação (consistência de padrão).
    /// </summary>
    public class ProductService : IProductService
    {
        public Task<IReadOnlyList<ProductResponse>> GetAllAsync()
        {
            var response = ProductCatalog.Products
                .Select(MapToResponse)
                .ToList();

            return Task.FromResult<IReadOnlyList<ProductResponse>>(response);
        }

        public Task<ProductResponse?> GetByIdAsync(Guid id)
        {
            var product = ProductCatalog.Products.FirstOrDefault(p => p.Id == id);

            return Task.FromResult(product is null ? null : MapToResponse(product));
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
        }
    }
}
