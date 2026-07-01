using ToyStore.Contracts.Responses;

namespace ToyStore.ApiGateway.Services
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductResponse>> GetAllAsync();
        Task<ProductResponse?> GetByIdAsync(Guid id);
    }
}
