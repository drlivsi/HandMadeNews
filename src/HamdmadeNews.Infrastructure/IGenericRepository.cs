using System.Linq.Expressions;

namespace HamdmadeNews.Infrastructure
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate);
        // Task<PaginatedList<T>> FindBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageIndex, int pageSize);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> Count(Expression<Func<T, bool>> predicate);
        IQueryable<T> Set();
    }
}