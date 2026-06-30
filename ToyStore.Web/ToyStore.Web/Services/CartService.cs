using ToyStore.Web.Dtos;
using ToyStore.Web.Models;

namespace ToyStore.Web.Services
{
    /// <summary>
    /// Gerencia o estado do carrinho em memória, por circuito Blazor (Scoped).
    /// Nenhum dado é persistido: ao reiniciar a aplicação ou encerrar a sessão,
    /// o carrinho é perdido — comportamento esperado nesta etapa.
    /// </summary>
    public class CartService
    {
        private readonly List<CartItem> _items = new();

        /// <summary>
        /// Disparado sempre que o carrinho muda, para que componentes
        /// como a Navbar possam se atualizar automaticamente.
        /// </summary>
        public event Action? OnChange;

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public void AddItem(ProductDto product, int quantity = 1)
        {
            if (quantity <= 0)
                quantity = 1;

            var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);

            if (existing is not null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }

            NotifyStateChanged();
        }

        public void RemoveItem(Guid productId)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == productId);

            if (existing is null)
                return;

            _items.Remove(existing);
            NotifyStateChanged();
        }

        public void UpdateQuantity(Guid productId, int quantity)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == productId);

            if (existing is null)
                return;

            if (quantity <= 0)
            {
                _items.Remove(existing);
            }
            else
            {
                existing.Quantity = quantity;
            }

            NotifyStateChanged();
        }

        public void Clear()
        {
            _items.Clear();
            NotifyStateChanged();
        }

        public decimal GetTotal() => _items.Sum(i => i.Total);

        public int GetItemCount() => _items.Sum(i => i.Quantity);

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
