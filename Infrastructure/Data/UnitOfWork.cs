using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
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
        private readonly Dictionary<Type, DbContext> _entityDbContextMap;
        private bool _disposed = false;

        // Optional: Inject ILogger<UnitOfWork> if you want to use logging
        // private readonly ILogger<UnitOfWork> _logger;
        // public UnitOfWork(FirstDbContext firstDbContext, SecondDbContext secondDbContext, ILogger<UnitOfWork> logger)
        // {
        //     _firstDbContext = firstDbContext;
        //     _secondDbContext = secondDbContext;
        //     _logger = logger;
        //     ...
        // }

        public UnitOfWork(FirstDbContext firstDbContext, SecondDbContext secondDbContext)
        {
            _firstDbContext = firstDbContext;
            _secondDbContext = secondDbContext;

            _entityDbContextMap = new Dictionary<Type, DbContext>
            {
                { typeof(Product), _firstDbContext },
                // เพิ่ม mapping อื่น ๆ ได้ที่นี่
            };
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (_repositories.ContainsKey(type))
                return (IGenericRepository<T>)_repositories[type];

            if (!_entityDbContextMap.TryGetValue(type, out var dbContext))
                throw new NotSupportedException($"No DbContext mapping for type {type.Name}");

            var repoInstance = new GenericRepository<T>(dbContext);
            _repositories[type] = repoInstance;
            return repoInstance;
        }

        public async Task CommitAsync()
        {
            try
            {
                await _firstDbContext.SaveChangesAsync();
                await _secondDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Logging example (uncomment if using ILogger)
                // _logger?.LogError(ex, "Error occurred during CommitAsync in UnitOfWork.");
                // หรือใช้ Console.WriteLine ชั่วคราว
                Console.WriteLine($"Error in CommitAsync: {ex.Message}");
                throw;
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