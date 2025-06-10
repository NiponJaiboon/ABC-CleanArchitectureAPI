using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Core.Attributes;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FirstDbContext _firstDbContext;
        private readonly SecondDbContext _secondDbContext;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed = false;

        public UnitOfWork(FirstDbContext firstDbContext, SecondDbContext secondDbContext)
        {
            _firstDbContext = firstDbContext;
            _secondDbContext = secondDbContext;
        }

        public IGenericRepository<T> Repository<T>()
            where T : class
        {
            var type = typeof(T);
            if (_repositories.ContainsKey(type))
                return (IGenericRepository<T>)_repositories[type];

            // อ่าน Attribute เพื่อเลือก DbContext
            var attr =
                type.GetCustomAttributes(typeof(DbContextNameAttribute), false).FirstOrDefault()
                as DbContextNameAttribute;

            DbContext dbContext = attr?.Name switch
            {
                "FirstDbContext" => _firstDbContext,
                "SecondDbContext" => _secondDbContext,
                _ => throw new NotSupportedException($"No DbContext mapping for type {type.Name}"),
            };

            var repoInstance = new GenericRepository<T>(dbContext);
            _repositories[type] = repoInstance;
            return repoInstance;
        }

        public async Task CommitAsync()
        {
            // ใช้ TransactionScope เพื่อให้แน่ใจว่าการเปลี่ยนแปลงในทั้งสอง DbContext จะถูก Commit หรือ Rollback พร้อมกัน
            // TransactionScopeAsyncFlowOption.Enabled ช่วยให้สามารถใช้ TransactionScope ใน async method ได้
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _firstDbContext.SaveChangesAsync();
                    await _secondDbContext.SaveChangesAsync();

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    // Logging example (uncomment if using ILogger)
                    // _logger?.LogError(ex, "Error occurred during CommitAsync in UnitOfWork.");
                    Console.WriteLine($"Error in CommitAsync: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Rollback changes in all tracked entities for both DbContexts.
        /// </summary>
        public Task RollbackAsync()
        {
            RollbackDbContext(_firstDbContext);
            RollbackDbContext(_secondDbContext);
            return Task.CompletedTask;
        }

        private void RollbackDbContext(DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _firstDbContext?.Dispose();
                    _secondDbContext?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
