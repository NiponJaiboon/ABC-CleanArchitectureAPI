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

        public async Task AddAsync(Product entity)
        {
            var res = _firstDbContext.Products.Add(entity);
            await _firstDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _firstDbContext.Products.FindAsync(id);
            if (product != null)
            {
                _firstDbContext.Products.Remove(product);
                await _firstDbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
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

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _firstDbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
        {
            return await _firstDbContext.Products
                .Where(p => p.Id == id)
                .ToListAsync();
        }

        public async Task<Product> GetProductWithDetailsAsync(int id)
        {
            return await _firstDbContext.Products
                .Include(p => p.Id)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product entity)
        {
            var trackedEntity = await _firstDbContext.Products.FindAsync(entity.Id);
            if (trackedEntity != null)
            {
                _firstDbContext.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _firstDbContext.Products.Update(entity);
            }
            await _firstDbContext.SaveChangesAsync();
        }
    }
}
