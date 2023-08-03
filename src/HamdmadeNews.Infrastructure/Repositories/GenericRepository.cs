using System.Linq.Expressions;
using HamdmadeNews.Infrastructure.Data;
using HandmadeNews.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace HamdmadeNews.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApplicationDbContext Context;

        private bool _disposed = false;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.Context = dbContext;
        }

        public async Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task Add(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
        }

        public Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate).CountAsync();
        }

        public IQueryable<T> Set()
        {
            return Context.Set<T>();
        }

        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
