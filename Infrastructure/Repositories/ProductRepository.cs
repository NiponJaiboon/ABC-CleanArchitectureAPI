using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Dapper;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                using (var connection = _firstDbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    var sql = "SELECT Id, Name, Description, Price, Stock, Remark FROM [ABC_DB].[dbo].[Products]";
                    var products = await connection.QueryAsync<Product>(sql);
                    return products;
                }
            }
            catch (Exception ex)
            {
                var exx = ex.Message;
                return new List<Product>();
            }
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _firstDbContext.Products.FindAsync(id);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var trackedEntity = await _firstDbContext.Products.FindAsync(product.Id);
            if (trackedEntity != null)
            {
                _firstDbContext.Entry(trackedEntity).CurrentValues.SetValues(product);
            }
            else
            {
                _firstDbContext.Products.Update(product);
            }
            await _firstDbContext.SaveChangesAsync();
        }
    }
}
