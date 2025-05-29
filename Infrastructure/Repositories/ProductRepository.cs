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
        private readonly FirstDbContext _dbContext;

        public ProductRepository(FirstDbContext firstDbContext)
        {
            _dbContext = firstDbContext;
        }

        public async Task AddAsync(Product entity)
        {
            var res = _dbContext.Products.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                using (var connection = _dbContext.Database.GetDbConnection())
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

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
        {
            return await _dbContext.Products
                .Where(p => p.Id == id)
                .ToListAsync();
        }

        public async Task<Product> GetProductWithDetailsAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Id)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product entity)
        {
            var trackedEntity = await _dbContext.Products.FindAsync(entity.Id);
            if (trackedEntity != null)
            {
                _dbContext.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _dbContext.Products.Update(entity);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
