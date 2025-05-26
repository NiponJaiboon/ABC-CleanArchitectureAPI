using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly FirstDbContext _firstDbContext;
        private readonly SecondDbContext _secondDbContext;
        public ProductRepository(FirstDbContext firstDbContext, SecondDbContext secondDbContext)
        {
            _firstDbContext = firstDbContext;
            _secondDbContext = secondDbContext;
        }
        public async Task AddProductAsync(Product product)
        {
            var res = _firstDbContext.Products.Add(product);
            await _firstDbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _firstDbContext.Products.FindAsync(id);
            if (product != null)
            {
                _firstDbContext.Products.Remove(product);
                await _firstDbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _firstDbContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _firstDbContext.Products.FindAsync(new { id });
        }

        public async Task UpdateProductAsync(Product product)
        {
            _firstDbContext.Products.Update(product);
            await _firstDbContext.SaveChangesAsync();
        }
    }
}
