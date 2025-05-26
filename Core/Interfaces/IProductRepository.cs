using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task DeleteProductAsync(int id);
    }
}
