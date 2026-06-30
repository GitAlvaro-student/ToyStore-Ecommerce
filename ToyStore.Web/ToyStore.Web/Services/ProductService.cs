using ToyStore.Web.Dtos;

namespace ToyStore.Web.Services
{
    /// <summary>
    /// Serviço responsável por consumir os endpoints de produtos da API.
    /// Toda comunicação HTTP do frontend relacionada a produtos passa por aqui.
    /// </summary>
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products");
                return products ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar a lista de produtos na API.");
                throw new ProductServiceException(
                    "Não foi possível carregar os produtos no momento. Tente novamente em instantes.",
                    ex);
            }
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar o produto {ProductId} na API.", id);
                throw new ProductServiceException(
                    "Não foi possível carregar este produto no momento. Tente novamente em instantes.",
                    ex);
            }
        }
    }
}
