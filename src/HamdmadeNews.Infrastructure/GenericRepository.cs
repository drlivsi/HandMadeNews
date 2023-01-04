using HamdmadeNews.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HamdmadeNews.Infrastructure
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

        //public async Task<PaginatedList<T>> FindBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageIndex, int pageSize)
        //{
        //    var query = Context.Set<T>().Where(predicate);
        //    query = orderBy(query);

        //    return await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);
        //}

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
