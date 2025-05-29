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
      
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
        {
            using (var connection = _dbContext.Database.GetDbConnection())
            {
                var sql = "SELECT * FROM Products WHERE Id = @CategoryId";
                var products = await connection.QueryAsync<Product>(sql, new { CategoryId = id });
                return products;
            }
        }

        public async Task<Product> GetProductWithDetailsAsync(int id)
        {
            using (var connection = _dbContext.Database.GetDbConnection())
            {
                var sql = "SELECT * FROM Products WHERE Id = @Id";
                var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
                return product;
            }
        }
       
    }
}
