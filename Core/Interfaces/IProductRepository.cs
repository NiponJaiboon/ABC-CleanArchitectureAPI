using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product> GetProductWithDetailsAsync(int id);
    }
}
